using System.Globalization;

namespace CopperSkin.Core.Theming;

public sealed class ThemeValidator
{
    private static readonly string[] RequiredTokens =
    [
        "color.surface.deep",
        "color.surface.panel",
        "color.surface.control",
        "color.accent.primary",
        "color.accent.light",
        "color.border.default",
        "color.text.primary",
        "color.text.secondary",
        "color.text.disabled"
    ];

    public IReadOnlyList<ThemeDiagnostic> Validate(ThemePack pack, bool strictContrast = false)
    {
        var diagnostics = new List<ThemeDiagnostic>();
        if (string.IsNullOrWhiteSpace(pack.Id))
            diagnostics.Add(new ThemeDiagnostic("Error", "PACK001", "Theme pack id is required.", "id"));

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (ThemeDefinition theme in pack.Themes)
        {
            string path = $"themes/{theme.Id}";
            if (string.IsNullOrWhiteSpace(theme.Id))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME001", "Theme id is required.", path));
            else if (!seen.Add(theme.Id))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME002", $"Duplicate theme id '{theme.Id}'.", path));

            if (string.IsNullOrWhiteSpace(theme.Name))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME003", "Theme name is required.", path));

            foreach (KeyValuePair<string, string> pair in theme.Tokens)
            {
                if (LooksColorToken(pair.Key) && !TryParseColor(pair.Value, out _))
                    diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN001", $"Token '{pair.Key}' is not a valid #RRGGBB or #AARRGGBB color.", $"{path}/tokens/{pair.Key}"));
            }

            ThemePack temp = new() { Themes = [theme.Clone()] };
            ResolvedTheme resolved;
            try
            {
                resolved = new ThemeResolver().Resolve(temp, theme.Id);
            }
            catch (Exception ex)
            {
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME004", ex.Message, path));
                continue;
            }

            foreach (string token in RequiredTokens)
            {
                if (!resolved.Tokens.ContainsKey(token))
                    diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN002", $"Required token '{token}' is missing.", $"{path}/tokens/{token}"));
            }

            AddContrastDiagnostics(resolved, diagnostics, path, strictContrast);
        }

        return diagnostics;
    }

    private static void AddContrastDiagnostics(ResolvedTheme theme, List<ThemeDiagnostic> diagnostics, string path, bool strict)
    {
        if (!TryParseColor(theme.Get("color.surface.panel"), out var panel) ||
            !TryParseColor(theme.Get("color.text.primary"), out var primary) ||
            !TryParseColor(theme.Get("color.text.secondary"), out var secondary))
        {
            return;
        }

        double primaryRatio = Contrast(panel, primary);
        double secondaryRatio = Contrast(panel, secondary);
        string severity = strict ? "Error" : "Warning";

        if (primaryRatio < 4.5)
            diagnostics.Add(new ThemeDiagnostic(severity, "A11Y001", $"Primary text contrast is {primaryRatio:0.00}:1; expected at least 4.5:1.", $"{path}/contrast/primary"));

        if (secondaryRatio < 3.0)
            diagnostics.Add(new ThemeDiagnostic(severity, "A11Y002", $"Secondary text contrast is {secondaryRatio:0.00}:1; expected at least 3.0:1.", $"{path}/contrast/secondary"));
    }

    public static bool TryParseColor(string value, out RgbaColor color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(value) || value[0] != '#')
            return false;

        string hex = value.Substring(1);
        if (hex.Length != 6 && hex.Length != 8)
            return false;

        if (!uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint raw))
            return false;

        if (hex.Length == 6)
            color = new RgbaColor(255, (byte)(raw >> 16), (byte)(raw >> 8), (byte)raw);
        else
            color = new RgbaColor((byte)(raw >> 24), (byte)(raw >> 16), (byte)(raw >> 8), (byte)raw);

        return true;
    }

    private static bool LooksColorToken(string key)
    {
        return key.StartsWith("color.", StringComparison.OrdinalIgnoreCase) ||
               LegacyTokenAliases.Map.ContainsKey(key) ||
               key.EndsWith("Color", StringComparison.OrdinalIgnoreCase);
    }

    private static double Contrast(RgbaColor a, RgbaColor b)
    {
        double l1 = Luminance(a) + 0.05;
        double l2 = Luminance(b) + 0.05;
        return Math.Max(l1, l2) / Math.Min(l1, l2);
    }

    private static double Luminance(RgbaColor color)
    {
        static double Channel(byte value)
        {
            double s = value / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Channel(color.R)) + (0.7152 * Channel(color.G)) + (0.0722 * Channel(color.B));
    }
}

public readonly record struct RgbaColor(byte A, byte R, byte G, byte B);
