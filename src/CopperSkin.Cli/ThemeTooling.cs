/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Cli\ThemeTooling.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 267FFE9F
 *
 *  Description    :
 *                   Implements gallery, baseline, diff, scaffold, adapter, and migration CLI helpers.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: 267FFE9F
// copperskin:allow-hardcoded-color-file
using System.Net;
using System.Text;
using System.Text.Json;
using CopperSkin.Core.Audit;
using CopperSkin.Core.Theming;

namespace CopperSkin.Cli;

/// <summary>
/// Implements higher-level CopperSkin authoring and release tooling.
/// </summary>
internal static class ThemeTooling
{

    private static string E(string value) => WebUtility.HtmlEncode(value);
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static void Scaffold(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        var tokens = ThemeTokenCatalog.Defaults.ToDictionary(static p => p.Key, static p => p.Value, StringComparer.OrdinalIgnoreCase);
        var pack = new ThemePack
        {
            Id = "my-company.copperskin",
            Name = "My CopperSkin Theme Pack",
            Version = "0.1.0",
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["author"] = "Your Team",
                ["compatibility.copperskin"] = "0.3.0.0"
            },
            Themes =
            [
                new ThemeDefinition
                {
                    Id = "my-theme",
                    Name = "My Theme",
                    Tokens = tokens
                }
            ]
        };

        ThemeJsonSerializer.WritePack(Path.Combine(outputDirectory, "theme-pack.json"), pack);
        File.WriteAllText(Path.Combine(outputDirectory, "README.md"), "# My CopperSkin Theme Pack\n\nEdit `theme-pack.json`, run `copperskin validate`, then package it with `copperskin pack`.\n");
    }

    private static string TextFor(string color) =>
        ThemeValidator.TryParseColor(color, out var parsed) && ((parsed.R * 0.299) + (parsed.G * 0.587) + (parsed.B * 0.114)) > 150
            ? "#111"
            : "#fff";

    public static void WriteAdapterCatalog(TextWriter writer)
    {
        writer.WriteLine("CopperSkin adapter targets");
        writer.WriteLine("  MaterialDesignInXaml : map palette helper brushes to CopperSkin color.* tokens");
        writer.WriteLine("  MahApps.Metro        : map accent, chrome, and dialog brushes");
        writer.WriteLine("  HandyControl         : map primary/secondary/status resources");
        writer.WriteLine("  AvalonDock           : map dock chrome, tabs, and document surface brushes");
        writer.WriteLine();
        writer.WriteLine("Use adapter.* tokens in packs for library-specific preferences, then keep canonical color/metric/font tokens as the source of truth.");
    }

    public static void WriteBaseline(ThemePack pack, string outputPath)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var resolver = new ThemeResolver();
        var model = pack.Themes.Select(theme =>
        {
            var resolved = resolver.Resolve(pack, theme.Id);
            return new
            {
                theme.Id,
                theme.Name,
                Signature = ThemePackSigner.ComputeHash(new ThemePack
                {
                    Id = pack.Id,
                    Name = pack.Name,
                    Version = pack.Version,
                    Themes = [theme.Clone()]
                }),
                Tokens = resolved.Tokens.OrderBy(static p => p.Key).ToDictionary(static p => p.Key, static p => p.Value)
            };
        }).ToArray();

        File.WriteAllText(outputPath, JsonSerializer.Serialize(model, JsonOptions));
    }

    public static int WriteDiff(ThemePack left, ThemePack right, TextWriter writer)
    {
        var resolver = new ThemeResolver();
        var rightById = right.Themes.ToDictionary(static t => t.Id, StringComparer.OrdinalIgnoreCase);
        var changes = 0;

        foreach (var leftTheme in left.Themes)
        {
            if (!rightById.TryGetValue(leftTheme.Id, out var rightTheme))
            {
                writer.WriteLine($"- Theme removed: {leftTheme.Id}");
                changes++;
                continue;
            }

            var leftTokens = resolver.Resolve(left, leftTheme.Id).Tokens;
            var rightTokens = resolver.Resolve(right, rightTheme.Id).Tokens;
            foreach (var key in leftTokens.Keys.Concat(rightTokens.Keys).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(static k => k))
            {
                leftTokens.TryGetValue(key, out var l);
                rightTokens.TryGetValue(key, out var r);
                if (!string.Equals(l, r, StringComparison.Ordinal))
                {
                    writer.WriteLine($"~ {leftTheme.Id} {key}: {l ?? "<missing>"} -> {r ?? "<missing>"}");
                    changes++;
                }
            }
        }

        foreach (var added in right.Themes.Where(theme => left.FindTheme(theme.Id) is null))
        {
            writer.WriteLine($"+ Theme added: {added.Id}");
            changes++;
        }

        return changes;
    }

    public static void WriteGallery(ThemePack pack, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        WriteBaseline(pack, Path.Combine(outputDirectory, "visual-baseline.json"));

        var resolver = new ThemeResolver();
        var html = new StringBuilder();
        html.AppendLine("<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
        html.AppendLine("<title>CopperSkin Theme Gallery</title>");
        html.AppendLine("<style>body{margin:0;font:14px Segoe UI,Arial,sans-serif;background:#111;color:#f5f5f5}.wrap{max-width:1280px;margin:auto;padding:24px}.theme{margin:0 0 24px;border:1px solid #444;border-radius:8px;overflow:hidden}.head{padding:16px 18px}.swatches{display:grid;grid-template-columns:repeat(auto-fit,minmax(130px,1fr));gap:1px;background:#333}.swatch{min-height:88px;padding:10px}.token{font-size:11px;opacity:.86;word-break:break-word}.sample{display:flex;gap:10px;align-items:center;padding:16px 18px;flex-wrap:wrap}.button{padding:8px 14px;border-radius:6px;border:1px solid currentColor}.input{padding:8px 12px;border-radius:6px;min-width:160px}</style></head><body><main class=\"wrap\">");
        html.Append("<h1>").Append(E(pack.Name)).Append("</h1><p>Version ").Append(E(pack.Version)).Append("</p>");

        foreach (var theme in pack.Themes)
        {
            var resolved = resolver.Resolve(pack, theme.Id);
            var deep = resolved.Get("color.surface.deep");
            var panel = resolved.Get("color.surface.panel");
            var text = resolved.Get("color.text.primary");
            var accent = resolved.Get("color.accent.primary");
            var border = resolved.Get("color.border.default");
            html.Append("<section class=\"theme\" style=\"background:").Append(E(deep)).Append(";color:").Append(E(text)).Append(";border-color:").Append(E(border)).Append("\">");
            html.Append("<div class=\"head\" style=\"background:").Append(E(panel)).Append("\"><h2>").Append(E(theme.Name)).Append("</h2><code>").Append(E(theme.Id)).Append("</code></div>");
            html.Append("<div class=\"sample\"><span class=\"button\" style=\"background:").Append(E(accent)).Append(";color:").Append(E(text)).Append("\">Primary</span>");
            html.Append("<span class=\"input\" style=\"background:").Append(E(resolved.Get("color.surface.control"))).Append(";border:1px solid ").Append(E(border)).Append("\">Input preview</span>");
            html.Append("<span>").Append(E(resolved.Get("font.ui", "Segoe UI"))).Append(" / radius ").Append(E(resolved.Get("metric.radius.md", "7"))).Append("</span></div>");
            html.Append("<div class=\"swatches\">");
            foreach (var pair in resolved.Tokens.Where(static p => p.Key.StartsWith("color.", StringComparison.OrdinalIgnoreCase)).OrderBy(static p => p.Key))
                html.Append("<div class=\"swatch\" style=\"background:").Append(E(pair.Value)).Append(";color:").Append(TextFor(pair.Value)).Append("\"><div class=\"token\">").Append(E(pair.Key)).Append("</div><strong>").Append(E(pair.Value)).Append("</strong></div>");
            html.Append("</div></section>");
        }

        html.AppendLine("</main></body></html>");
        File.WriteAllText(Path.Combine(outputDirectory, "index.html"), html.ToString());
    }

    public static void WriteMigrationReport(string root, string reportPath)
    {
        var findings = HardCodedColorAudit.ScanDirectory(root);
        var directory = Path.GetDirectoryName(reportPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var builder = new StringBuilder();
        builder.AppendLine("# CopperSkin Migration Report").AppendLine();
        builder.AppendLine($"Source: `{root}`").AppendLine();
        builder.AppendLine($"Findings: {findings.Count}").AppendLine();
        builder.AppendLine("## Suggested First Tokens").AppendLine();
        builder.AppendLine("- Backgrounds: `color.surface.deep`, `color.surface.panel`, `color.surface.control`");
        builder.AppendLine("- Text: `color.text.primary`, `color.text.secondary`, `color.text.disabled`");
        builder.AppendLine("- Accent/borders: `color.accent.primary`, `color.accent.light`, `color.border.default`").AppendLine();

        foreach (var finding in findings)
            builder.AppendLine($"- `{finding.File}:{finding.Line}:{finding.Column}` {finding.Code}: {finding.Message}");

        File.WriteAllText(reportPath, builder.ToString());
    }

    public static void WriteTokenCatalog(TextWriter writer)
    {
        foreach (var token in ThemeTokenCatalog.All.OrderBy(static t => t.Group).ThenBy(static t => t.Key))
            writer.WriteLine($"{token.Group,-12} {token.Type,-10} {(token.Required ? "required" : "optional"),-8} {token.Key,-36} {token.DefaultValue}");
    }
}
