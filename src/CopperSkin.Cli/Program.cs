/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Cli\Program.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:31 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : AF53EB57
 *
 *  Description    :
 *                   CopperSkin WPF theme engine source file with live theming, custom controls, and designer support.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: AF53EB57
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using CopperSkin.Core.Audit;
using CopperSkin.Core.Theming;
using QuickLog;

namespace CopperSkin.Cli;

/// <summary>
/// Implements CopperSkin command-line tooling for theme export, validation, auditing, packaging, and release archives.
/// </summary>
public static class Program
{
    /// <summary>
    /// Runs the CopperSkin CLI and returns a process-style exit code.
    /// </summary>
    public static int Main(string[] args)
    {
        string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CopperSkin", "CliLogs");
        LogManager.ConfigureDefault(LoggerOptions.ForTool("copperskin").WithJsonLog(Path.Combine(logDir, "copperskin-cli.jsonl")));
        try
        {
            return Run(args);
        }
        catch (Exception ex)
        {
            LogManager.GetDefaultLogger().Log(LogType.Error, "CopperSkin CLI failed", ex);
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    /// <summary>
    /// Dispatches command-line arguments to the requested CopperSkin maintenance command.
    /// </summary>
    private static int Run(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            PrintHelp();
            return 0;
        }

        return args[0].ToLowerInvariant() switch
        {
            "list" => ListThemes(),
            "export-builtins" => ExportBuiltins(Arg(args, 1, "themes/amchipper/theme-pack.json")),
            "validate" => Validate(Arg(args, 1, "themes/amchipper/theme-pack.json")),
            "audit" => Audit(Arg(args, 1, ".")),
            "pack" => Pack(Arg(args, 1, "."), Arg(args, 2, "artifacts/theme.cskin")),
            "unpack" => Unpack(Arg(args, 1, "artifacts/theme.cskin"), Arg(args, 2, "artifacts/unpacked")),
            "lzhc" => Lzhc(Arg(args, 1, "."), Arg(args, 2, "artifacts/package.lzhc")),
            _ => Unknown(args[0])
        };
    }

    /// <summary>
    /// Prints all bundled theme names to standard output.
    /// </summary>
    private static int ListThemes()
    {
        foreach (string name in BuiltInThemeCatalog.ThemeNames)
            Console.WriteLine(name);
        return 0;
    }

    /// <summary>
    /// Writes the built-in amChipper-inspired theme pack to a JSON file.
    /// </summary>
    private static int ExportBuiltins(string path)
    {
        ThemeJsonSerializer.WritePack(path, BuiltInThemeCatalog.Create());
        Console.WriteLine($"Wrote {path}");
        return 0;
    }

    /// <summary>
    /// Validates a theme pack and returns all diagnostics without mutating the pack.
    /// </summary>
    private static int Validate(string path)
    {
        ThemePack pack = File.Exists(path) ? ThemeJsonSerializer.ReadPack(path) : BuiltInThemeCatalog.Create();
        IReadOnlyList<ThemeDiagnostic> diagnostics = new ThemeValidator().Validate(pack);
        foreach (ThemeDiagnostic diagnostic in diagnostics)
            Console.WriteLine(diagnostic);

        int errors = diagnostics.Count(static d => d.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"Validated {pack.Themes.Count} theme(s), {errors} error(s), {diagnostics.Count - errors} warning(s).");
        return errors == 0 ? 0 : 2;
    }

    /// <summary>
    /// Scans a WPF source tree for hard-coded colors that bypass CopperSkin tokens.
    /// </summary>
    private static int Audit(string root)
    {
        IReadOnlyList<AuditFinding> findings = HardCodedColorAudit.ScanDirectory(root);
        foreach (AuditFinding finding in findings)
            Console.WriteLine($"{finding.File}:{finding.Line}:{finding.Column} {finding.Code} {finding.Message}");

        Console.WriteLine($"Audit complete: {findings.Count} hard-coded color finding(s).");
        return 0;
    }

    /// <summary>
    /// Packages a theme directory into the portable CopperSkin .cskin archive format.
    /// </summary>
    private static int Pack(string sourceDirectory, string outputPath)
    {
        ThemePackArchive.PackDirectory(sourceDirectory, outputPath);
        Console.WriteLine($"Packed {sourceDirectory} -> {outputPath}");
        return 0;
    }

    /// <summary>
    /// Extracts a .cskin archive to a target directory.
    /// </summary>
    private static int Unpack(string archivePath, string outputDirectory)
    {
        ThemePackArchive.Unpack(archivePath, outputDirectory);
        Console.WriteLine($"Unpacked {archivePath} -> {outputDirectory}");
        return 0;
    }

    /// <summary>
    /// Compresses a release directory into CopperSkin's custom LZHC-style archive.
    /// </summary>
    private static int Lzhc(string sourceDirectory, string outputPath)
    {
        LzhcArchive.Create(sourceDirectory, outputPath);
        Console.WriteLine($"LZHC packed {sourceDirectory} -> {outputPath}");
        return 0;
    }

    /// <summary>
    /// Reports an unsupported command and prints the help text.
    /// </summary>
    private static int Unknown(string command)
    {
        Console.Error.WriteLine($"Unknown command '{command}'.");
        PrintHelp();
        return 1;
    }

    /// <summary>
    /// Reads an optional positional argument with a default fallback.
    /// </summary>
    private static string Arg(string[] args, int index, string fallback)
    {
        return args.Length > index ? args[index] : fallback;
    }

    /// <summary>
    /// Prints the supported CopperSkin CLI commands.
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine("CopperSkin CLI");
        Console.WriteLine("  list");
        Console.WriteLine("  export-builtins <theme-pack.json>");
        Console.WriteLine("  validate <theme-pack.json>");
        Console.WriteLine("  audit <wpf-source-root>");
        Console.WriteLine("  pack <directory> <out.cskin>");
        Console.WriteLine("  unpack <in.cskin> <directory>");
        Console.WriteLine("  lzhc <directory> <out.lzhc>");
    }
}

/// <summary>
/// Writes the current CopperSkin custom LZHC-style release archive format.
/// </summary>
internal static class LzhcArchive
{
    private const string Magic = "CSLZHC1";

    /// <summary>
    /// Creates a deterministic compressed release archive from every file in a source directory.
    /// </summary>
    public static void Create(string sourceDirectory, string outputPath)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException(sourceDirectory);

        string? directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var manifest = Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories)
            .Select(path => new LzhcEntry(
                Path.GetRelativePath(sourceDirectory, path).Replace('\\', '/'),
                File.ReadAllBytes(path)))
            .OrderBy(static e => e.Path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        byte[] raw = JsonSerializer.SerializeToUtf8Bytes(manifest.Select(static e => new SerializableEntry(e.Path, Convert.ToBase64String(e.Bytes))).ToArray());
        using var file = File.Create(outputPath);
        using var writer = new BinaryWriter(file, Encoding.UTF8, leaveOpen: true);
        writer.Write(Encoding.ASCII.GetBytes(Magic));
        writer.Write(raw.Length);
        using var brotli = new BrotliStream(file, CompressionLevel.SmallestSize, leaveOpen: true);
        brotli.Write(raw, 0, raw.Length);
    }

    /// <summary>
    /// Stores one in-memory release file before it is serialized into the LZHC payload.
    /// </summary>
    private sealed record LzhcEntry(string Path, byte[] Bytes);

    /// <summary>
    /// Stores one base64-encoded archive entry in the JSON manifest inside the compressed payload.
    /// </summary>
    private sealed record SerializableEntry(string Path, string Data);
}
