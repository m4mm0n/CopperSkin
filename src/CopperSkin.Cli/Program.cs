/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Cli\Program.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:31 +02:00
 *  Last Modified  : 2026-07-03 09:57:50 +02:00
 *  CRC32          : 0CF752A0
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
// CRC32-BODY: 0CF752A0

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
    /// Prints adapter guidance for common WPF ecosystems.
    /// </summary>
    private static int Adapters()
    {
        ThemeTooling.WriteAdapterCatalog(Console.Out);
        return 0;
    }

    /// <summary>
    /// Reads an optional positional argument with a default fallback.
    /// </summary>
    private static string Arg(string[] args, int index, string fallback) => args.Length > index ? args[index] : fallback;

    /// <summary>
    /// Scans a WPF source tree for hard-coded colors that bypass CopperSkin tokens.
    /// </summary>
    private static int Audit(string root)
    {
        var findings = HardCodedColorAudit.ScanDirectory(root);
        foreach (var finding in findings)
            Console.WriteLine($"{finding.File}:{finding.Line}:{finding.Column} {finding.Code} {finding.Message}");

        Console.WriteLine($"Audit complete: {findings.Count} hard-coded color finding(s).");
        return 0;
    }

    /// <summary>
    /// Writes a deterministic theme token baseline for visual regression workflows.
    /// </summary>
    private static int Baseline(string packPath, string outputPath)
    {
        ThemeTooling.WriteBaseline(ReadPack(packPath), outputPath);
        Console.WriteLine($"Baseline written to {outputPath}");
        return 0;
    }

    /// <summary>
    /// Compares two theme packs and writes token differences.
    /// </summary>
    private static int Diff(string leftPath, string rightPath)
    {
        var changes = ThemeTooling.WriteDiff(ReadPack(leftPath), ReadPack(rightPath), Console.Out);
        Console.WriteLine($"Diff complete: {changes} change(s).");
        return changes == 0 ? 0 : 3;
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
    /// Generates a static theme gallery and visual baseline manifest.
    /// </summary>
    private static int Gallery(string packPath, string outputDirectory)
    {
        ThemeTooling.WriteGallery(ReadPack(packPath), outputDirectory);
        Console.WriteLine($"Gallery written to {outputDirectory}");
        return 0;
    }

    /// <summary>
    /// Generates a reusable B-233 signing key pair.
    /// </summary>
    private static int Keygen(string privateKeyPath, string publicKeyPath)
    {
        var keyPair = ThemePackSigner.CreateKeyPair();
        WriteSecret(privateKeyPath, keyPair.PrivateKeyHex);
        WriteText(publicKeyPath, keyPair.PublicKeyHex);
        Console.WriteLine($"Private key written to {privateKeyPath}");
        Console.WriteLine($"Public key written to {publicKeyPath}");
        return 0;
    }

    /// <summary>
    /// Prints all bundled theme names to standard output.
    /// </summary>
    private static int ListThemes()
    {
        foreach (var name in BuiltInThemeCatalog.ThemeNames)
            Console.WriteLine(name);
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
    /// Runs the CopperSkin CLI and returns a process-style exit code.
    /// </summary>
    public static int Main(string[] args)
    {
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CopperSkin", "CliLogs");
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
    /// Writes a markdown migration report with token replacement guidance.
    /// </summary>
    private static int Migrate(string root, string reportPath)
    {
        ThemeTooling.WriteMigrationReport(root, reportPath);
        Console.WriteLine($"Migration report written to {reportPath}");
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
    /// Prints the supported CopperSkin CLI commands.
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine("CopperSkin CLI");
        Console.WriteLine("  list");
        Console.WriteLine("  tokens");
        Console.WriteLine("  export-builtins <theme-pack.json>");
        Console.WriteLine("  validate <theme-pack.json>");
        Console.WriteLine("  audit <wpf-source-root>");
        Console.WriteLine("  migrate <wpf-source-root> <report.md>");
        Console.WriteLine("  gallery <theme-pack.json> <directory>");
        Console.WriteLine("  baseline <theme-pack.json> <visual-baseline.json>");
        Console.WriteLine("  diff <left-theme-pack.json> <right-theme-pack.json>");
        Console.WriteLine("  scaffold <directory>");
        Console.WriteLine("  keygen <private.key> <public.key>");
        Console.WriteLine("  sign <theme-pack.json> [out.json] [private.key]");
        Console.WriteLine("  verify-signature <theme-pack.json> [public.key]");
        Console.WriteLine("  adapters");
        Console.WriteLine("  pack <directory> <out.cskin>");
        Console.WriteLine("  unpack <in.cskin> <directory>");
        Console.WriteLine("  lzhc <directory> <out.lzhc>");
    }

    private static ThemePack ReadPack(string path) => File.Exists(path) ? ThemeJsonSerializer.ReadPack(path) : BuiltInThemeCatalog.Create();

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
            "tokens" => Tokens(),
            "export-builtins" => ExportBuiltins(Arg(args, 1, "themes/amchipper/theme-pack.json")),
            "validate" => Validate(Arg(args, 1, "themes/amchipper/theme-pack.json")),
            "audit" => Audit(Arg(args, 1, ".")),
            "migrate" => Migrate(Arg(args, 1, "."), Arg(args, 2, "artifacts/copperskin-migration.md")),
            "gallery" => Gallery(Arg(args, 1, "themes/amchipper/theme-pack.json"), Arg(args, 2, "artifacts/gallery")),
            "baseline" => Baseline(Arg(args, 1, "themes/amchipper/theme-pack.json"), Arg(args, 2, "artifacts/visual-baseline.json")),
            "diff" => Diff(Arg(args, 1, "themes/amchipper/theme-pack.json"), Arg(args, 2, "themes/amchipper/theme-pack.json")),
            "scaffold" => Scaffold(Arg(args, 1, "artifacts/starter-theme")),
            "keygen" => Keygen(Arg(args, 1, "artifacts/copperskin-signing.private"), Arg(args, 2, "artifacts/copperskin-signing.public")),
            "sign" => Sign(Arg(args, 1, "themes/amchipper/theme-pack.json"), Arg(args, 2, string.Empty), Arg(args, 3, string.Empty)),
            "verify-signature" => VerifySignature(Arg(args, 1, "themes/amchipper/theme-pack.json"), Arg(args, 2, string.Empty)),
            "adapters" => Adapters(),
            "pack" => Pack(Arg(args, 1, "."), Arg(args, 2, "artifacts/theme.cskin")),
            "unpack" => Unpack(Arg(args, 1, "artifacts/theme.cskin"), Arg(args, 2, "artifacts/unpacked")),
            "lzhc" => Lzhc(Arg(args, 1, "."), Arg(args, 2, "artifacts/package.lzhc")),
            _ => Unknown(args[0])
        };
    }

    /// <summary>
    /// Creates a starter theme pack with every recommended CopperSkin token.
    /// </summary>
    private static int Scaffold(string outputDirectory)
    {
        ThemeTooling.Scaffold(outputDirectory);
        Console.WriteLine($"Starter theme scaffold written to {outputDirectory}");
        return 0;
    }

    /// <summary>
    /// Signs a theme pack using a B-233 binary-field ECDSA metadata signature.
    /// </summary>
    private static int Sign(string packPath, string outputPath, string privateKeyPath)
    {
        outputPath = string.IsNullOrWhiteSpace(outputPath) ? packPath : outputPath;
        var pack = ReadPack(packPath);
        var privateKey = string.IsNullOrWhiteSpace(privateKeyPath) ? null : File.ReadAllText(privateKeyPath).Trim();
        var signature = ThemePackSigner.Sign(pack, privateKeyHex: privateKey);
        ThemeJsonSerializer.WritePack(outputPath, pack);
        Console.WriteLine($"Signed {outputPath} with {pack.Metadata[ThemePackSigner.AlgorithmKey]} {signature}");
        return 0;
    }

    /// <summary>
    /// Prints the known CopperSkin token catalog.
    /// </summary>
    private static int Tokens()
    {
        ThemeTooling.WriteTokenCatalog(Console.Out);
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
    /// Extracts a .cskin archive to a target directory.
    /// </summary>
    private static int Unpack(string archivePath, string outputDirectory)
    {
        ThemePackArchive.Unpack(archivePath, outputDirectory);
        Console.WriteLine($"Unpacked {archivePath} -> {outputDirectory}");
        return 0;
    }

    /// <summary>
    /// Validates a theme pack and returns all diagnostics without mutating the pack.
    /// </summary>
    private static int Validate(string path)
    {
        var pack = File.Exists(path) ? ThemeJsonSerializer.ReadPack(path) : BuiltInThemeCatalog.Create();
        var diagnostics = new ThemeValidator().Validate(pack);
        foreach (var diagnostic in diagnostics)
            Console.WriteLine(diagnostic);

        var errors = diagnostics.Count(static d => d.Severity.Equals("Error", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"Validated {pack.Themes.Count} theme(s), {errors} error(s), {diagnostics.Count - errors} warning(s).");
        return errors == 0 ? 0 : 2;
    }

    /// <summary>
    /// Verifies a signed theme pack.
    /// </summary>
    private static int VerifySignature(string packPath, string publicKeyPath)
    {
        var trustedPublicKey = string.IsNullOrWhiteSpace(publicKeyPath) ? null : File.ReadAllText(publicKeyPath).Trim();
        var valid = ThemePackSigner.Verify(ReadPack(packPath), trustedPublicKey);
        Console.WriteLine(valid ? "Signature valid." : "Signature missing or invalid.");
        return valid ? 0 : 4;
    }

    private static void WriteSecret(string path, string value)
    {
        WriteText(path, value);
        try
        {
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static void WriteText(string path, string value)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllText(path, value);
    }
}

/// <summary>
/// Writes the current CopperSkin custom LZHC-style release archive format.
/// </summary>
internal static class LzhcArchive
{

    /// <summary>
    /// Creates a deterministic compressed release archive from every file in a source directory.
    /// </summary>
    public static void Create(string sourceDirectory, string outputPath)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException(sourceDirectory);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var manifest = Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories)
            .Select(path => new LzhcEntry(
                Path.GetRelativePath(sourceDirectory, path).Replace('\\', '/'),
                File.ReadAllBytes(path)))
            .OrderBy(static e => e.Path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var raw = JsonSerializer.SerializeToUtf8Bytes(manifest.Select(static e => new SerializableEntry(e.Path, Convert.ToBase64String(e.Bytes))).ToArray());
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
    private const string Magic = "CSLZHC1";

    /// <summary>
    /// Stores one base64-encoded archive entry in the JSON manifest inside the compressed payload.
    /// </summary>
    private sealed record SerializableEntry(string Path, string Data);
}
