/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Audit\HardCodedColorAudit.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : B5786889
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
// CRC32-BODY: B5786889
using System.Text.RegularExpressions;

namespace CopperSkin.Core.Audit;

/// <summary>
/// Describes one source location where a hard-coded color bypasses CopperSkin theme tokens.
/// </summary>
public sealed record AuditFinding(string File, int Line, int Column, string Code, string Message);

/// <summary>
/// Scans C# and XAML source trees for color literals that should be represented by CopperSkin theme tokens.
/// </summary>
public static class HardCodedColorAudit
{
    /// <summary>
    /// Matches common WPF code color factories and stock brushes used outside token resources.
    /// </summary>
    private static readonly Regex CodeColor = new("(Color\\.From(?:Argb|Rgb)\\(|Brushes\\.[A-Z][A-Za-z]+)", RegexOptions.Compiled);
    /// <summary>
    /// Matches literal hexadecimal colors that should normally flow through theme tokens.
    /// </summary>
    private static readonly Regex HexColor = new("#[0-9A-Fa-f]{6,8}", RegexOptions.Compiled);

    /// <summary>
    /// Scans every C# and XAML file under a directory and returns hard-coded color findings.
    /// </summary>
    public static IReadOnlyList<AuditFinding> ScanDirectory(string root)
    {
        if (!Directory.Exists(root))
            throw new DirectoryNotFoundException(root);

        var files = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
            .Where(static path => path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var findings = new List<AuditFinding>();
        foreach (var file in files)
            findings.AddRange(ScanFile(file));

        return findings;
    }

    /// <summary>
    /// Scans one source file and returns hard-coded color findings not explicitly allowed by a CopperSkin marker.
    /// </summary>
    public static IReadOnlyList<AuditFinding> ScanFile(string path)
    {
        var lines = File.ReadAllLines(path);
        if (lines.Any(static line => line.IndexOf("copperskin:allow-hardcoded-color-file", StringComparison.OrdinalIgnoreCase) >= 0))
            return Array.Empty<AuditFinding>();

        var findings = new List<AuditFinding>();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.IndexOf("copperskin:allow-hardcoded-color", StringComparison.OrdinalIgnoreCase) >= 0)
                continue;

            findings.AddRange(HexColor.Matches(line).Cast<Match>().Concat(CodeColor.Matches(line).Cast<Match>())
                .Select(match => new AuditFinding(path, i + 1, match.Index + 1, "CSKIN001",
                    $"Hard-coded color '{match.Value}' should be mapped to a CopperSkin token.")));
        }

        return findings;
    }
}
