using System.Text.RegularExpressions;

namespace CopperSkin.Core.Audit;

public sealed record AuditFinding(string File, int Line, int Column, string Code, string Message);

public static class HardCodedColorAudit
{
    private static readonly Regex HexColor = new("#[0-9A-Fa-f]{6,8}", RegexOptions.Compiled);
    private static readonly Regex CodeColor = new("(Color\\.From(?:Argb|Rgb)\\(|Brushes\\.[A-Z][A-Za-z]+)", RegexOptions.Compiled);

    public static IReadOnlyList<AuditFinding> ScanDirectory(string root)
    {
        if (!Directory.Exists(root))
            throw new DirectoryNotFoundException(root);

        var files = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
            .Where(static path => path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var findings = new List<AuditFinding>();
        foreach (string file in files)
            findings.AddRange(ScanFile(file));

        return findings;
    }

    public static IReadOnlyList<AuditFinding> ScanFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        if (lines.Any(static line => line.IndexOf("copperskin:allow-hardcoded-color-file", StringComparison.OrdinalIgnoreCase) >= 0))
            return Array.Empty<AuditFinding>();

        var findings = new List<AuditFinding>();
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.IndexOf("copperskin:allow-hardcoded-color", StringComparison.OrdinalIgnoreCase) >= 0)
                continue;

            foreach (Match match in HexColor.Matches(line).Cast<Match>().Concat(CodeColor.Matches(line).Cast<Match>()))
            {
                findings.Add(new AuditFinding(path, i + 1, match.Index + 1, "CSKIN001", $"Hard-coded color '{match.Value}' should be mapped to a CopperSkin token."));
            }
        }

        return findings;
    }
}
