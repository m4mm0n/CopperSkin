/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeValidator.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 3F66CDE8
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
// CRC32-BODY: 3F66CDE8
using System.Globalization;
using CopperSkin.Core.Graphics;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Validates CopperSkin theme packs for identity, color syntax, required tokens, inheritance, and contrast.
/// </summary>
public sealed class ThemeValidator
{

    /// <summary>
    /// Adds WCAG-style contrast diagnostics for primary and secondary text tokens.
    /// </summary>
    private static void AddContrastDiagnostics(ResolvedTheme theme, List<ThemeDiagnostic> diagnostics, string path, bool strict)
    {
        if (!TryParseColor(theme.Get("color.surface.panel"), out var panel) ||
            !TryParseColor(theme.Get("color.text.primary"), out var primary) ||
            !TryParseColor(theme.Get("color.text.secondary"), out var secondary))
            return;

        var primaryRatio = Contrast(panel, primary);
        var secondaryRatio = Contrast(panel, secondary);
        var severity = strict ? "Error" : "Warning";

        if (primaryRatio < 4.5)
            diagnostics.Add(new ThemeDiagnostic(severity, "A11Y001", $"Primary text contrast is {primaryRatio:0.00}:1; expected at least 4.5:1.", $"{path}/contrast/primary"));

        if (secondaryRatio < 3.0)
            diagnostics.Add(new ThemeDiagnostic(severity, "A11Y002", $"Secondary text contrast is {secondaryRatio:0.00}:1; expected at least 3.0:1.", $"{path}/contrast/secondary"));

        if (TryParseColor(theme.Get("color.text.disabled"), out var disabled))
        {
            var disabledRatio = Contrast(panel, disabled);
            if (disabledRatio < 1.8)
                diagnostics.Add(new ThemeDiagnostic(severity, "A11Y003", $"Disabled text contrast is {disabledRatio:0.00}:1; expected at least 1.8:1.", $"{path}/contrast/disabled"));
        }

        if (TryParseColor(theme.Get("color.status.danger"), out var danger))
        {
            var dangerRatio = Contrast(panel, danger);
            if (dangerRatio < 3.0)
                diagnostics.Add(new ThemeDiagnostic(severity, "A11Y004", $"Danger status contrast is {dangerRatio:0.00}:1; expected at least 3.0:1.", $"{path}/contrast/danger"));
        }
    }

    /// <summary>
    /// Calculates the contrast ratio between two RGBA colors.
    /// </summary>
    private static double Contrast(RgbaColor a, RgbaColor b)
    {
        var l1 = Luminance(a) + 0.05;
        var l2 = Luminance(b) + 0.05;
        return Math.Max(l1, l2) / Math.Min(l1, l2);
    }

    /// <summary>
    /// Determines whether a token key should contain a parseable color value.
    /// </summary>
    private static bool LooksColorToken(string key) =>
        key.StartsWith("color.", StringComparison.OrdinalIgnoreCase) ||
        LegacyTokenAliases.Map.ContainsKey(key) ||
        key.EndsWith("Color", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Calculates relative luminance for one RGBA color using sRGB channel conversion.
    /// </summary>
    private static double Luminance(RgbaColor color)
    {
        static double Channel(byte value)
        {
            var s = value / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Channel(color.R)) + (0.7152 * Channel(color.G)) + (0.0722 * Channel(color.B));
    }

    /// <summary>
    /// Parses a #RRGGBB or #AARRGGBB token value into RGBA channels.
    /// </summary>
    public static bool TryParseColor(string value, out RgbaColor color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(value) || value[0] != '#')
            return false;

        var hex = value.Substring(1);
        if (hex.Length != 6 && hex.Length != 8)
            return false;

        if (!uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var raw))
            return false;

        if (hex.Length == 6)
            color = new RgbaColor(255, (byte)(raw >> 16), (byte)(raw >> 8), (byte)raw);
        else
            color = new RgbaColor((byte)(raw >> 24), (byte)(raw >> 16), (byte)(raw >> 8), (byte)raw);

        return true;
    }
    /// <summary>
    /// Validates a theme pack and returns all diagnostics without mutating the pack.
    /// </summary>
    public IReadOnlyList<ThemeDiagnostic> Validate(ThemePack pack, bool strictContrast = false)
    {
        return Validate(pack, new ThemeValidationOptions { StrictContrast = strictContrast });
    }

    /// <summary>
    /// Validates a theme pack and returns all diagnostics without mutating the pack.
    /// </summary>
    public IReadOnlyList<ThemeDiagnostic> Validate(ThemePack pack, ThemeValidationOptions? options)
    {
        var diagnostics = new List<ThemeDiagnostic>();
        options ??= new ThemeValidationOptions();
        if (string.IsNullOrWhiteSpace(pack.Id))
            diagnostics.Add(new ThemeDiagnostic("Error", "PACK001", "Theme pack id is required.", "id"));
        if (string.IsNullOrWhiteSpace(pack.Name))
            diagnostics.Add(new ThemeDiagnostic("Warning", "PACK002", "Theme pack name is recommended.", "name"));
        if (string.IsNullOrWhiteSpace(pack.Version))
            diagnostics.Add(new ThemeDiagnostic("Warning", "PACK003", "Theme pack version is recommended.", "version"));

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var theme in pack.Themes)
        {
            var path = $"themes/{theme.Id}";
            if (string.IsNullOrWhiteSpace(theme.Id))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME001", "Theme id is required.", path));
            else if (!seen.Add(theme.Id))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME002", $"Duplicate theme id '{theme.Id}'.", path));

            if (string.IsNullOrWhiteSpace(theme.Name))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME003", "Theme name is required.", path));

            if (!string.IsNullOrWhiteSpace(theme.BaseThemeId) && string.Equals(theme.BaseThemeId, theme.Id, StringComparison.OrdinalIgnoreCase))
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME005", $"Theme '{theme.Id}' cannot inherit from itself.", $"{path}/baseThemeId"));

            foreach (var pair in theme.Tokens)
                ValidateToken(pair.Key, pair.Value, diagnostics, $"{path}/tokens/{pair.Key}", options);

            ResolvedTheme resolved;
            try
            {
                resolved = new ThemeResolver().Resolve(pack, theme.Id);
            }
            catch (Exception ex)
            {
                diagnostics.Add(new ThemeDiagnostic("Error", "THEME004", ex.Message, path));
                continue;
            }

            diagnostics.AddRange(from token in ThemeTokenCatalog.RequiredKeys
                where !resolved.Tokens.ContainsKey(token)
                select new ThemeDiagnostic("Error", "TOKEN002", $"Required token '{token}' is missing.",
                    $"{path}/tokens/{token}"));

            if (options.RequireRecommendedTokens)
                diagnostics.AddRange(from token in ThemeTokenCatalog.All.Where(static d => d.Recommended).Select(static d => d.Key)
                                     where !resolved.Tokens.ContainsKey(token)
                                     select new ThemeDiagnostic("Warning", "TOKEN004", $"Recommended token '{token}' is missing.",
                                         $"{path}/tokens/{token}"));

            AddContrastDiagnostics(resolved, diagnostics, path, options.StrictContrast);
        }

        foreach (var graphic in pack.Graphics ?? [])
        {
            diagnostics.AddRange(GraphicDocumentValidator.Validate(graphic)
                .Select(diagnostic => new ThemeDiagnostic(diagnostic.Severity, diagnostic.Code, diagnostic.Message, $"graphics/{graphic.Id}/{diagnostic.Path}")));
        }

        return diagnostics;
    }

    private static void ValidateToken(string key, string value, List<ThemeDiagnostic> diagnostics, string path, ThemeValidationOptions options)
    {
        var canonical = LegacyTokenAliases.Canonicalize(key);
        if (!ThemeTokenCatalog.TryGet(canonical, out var definition))
        {
            if (options.WarnUnknownTokens)
                diagnostics.Add(new ThemeDiagnostic("Warning", "TOKEN003", $"Unknown token '{key}'.", path));
            if (LooksColorToken(canonical) && !TryParseColor(value, out _))
                diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN001", $"Token '{key}' is not a valid #RRGGBB or #AARRGGBB color.", path));
            return;
        }

        switch (definition.Type)
        {
            case ThemeTokenType.Color:
                if (!TryParseColor(value, out _))
                    diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN001", $"Token '{key}' is not a valid #RRGGBB or #AARRGGBB color.", path));
                break;
            case ThemeTokenType.Number:
            case ThemeTokenType.Duration:
                if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) || number < 0)
                    diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN005", $"Token '{key}' must be a non-negative number.", path));
                break;
            case ThemeTokenType.Opacity:
                if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var opacity) || opacity < 0 || opacity > 1)
                    diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN006", $"Token '{key}' must be an opacity from 0.0 through 1.0.", path));
                break;
            case ThemeTokenType.FontFamily:
            case ThemeTokenType.String:
                if (string.IsNullOrWhiteSpace(value))
                    diagnostics.Add(new ThemeDiagnostic("Error", "TOKEN007", $"Token '{key}' cannot be empty.", path));
                break;
        }
    }
}

/// <summary>
/// Represents a parsed color token as alpha, red, green, and blue channels.
/// </summary>
public readonly record struct RgbaColor(byte A, byte R, byte G, byte B);
