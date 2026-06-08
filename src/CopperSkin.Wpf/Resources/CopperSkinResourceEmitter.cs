/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Resources\CopperSkinResourceEmitter.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 0F827D68
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
// CRC32-BODY: 0F827D68
// copperskin:allow-hardcoded-color-file
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf.Resources;

/// <summary>
/// Turns resolved CopperSkin theme tokens into WPF colors, brushes, gradients, effects, and legacy resource aliases.
/// </summary>
public static class CopperSkinResourceEmitter
{
    private static readonly string[] DictionarySources =
    [
        "/CopperSkin.Wpf;component/Themes/CopperSkin.Controls.xaml"
    ];

    /// <summary>
    /// Merges the bundled CopperSkin control style dictionaries into a WPF resource dictionary.
    /// </summary>
    public static void MergeDefaultDictionaries(ResourceDictionary target)
    {
        foreach (var source in DictionarySources)
        {
            var uri = new Uri(source, UriKind.Relative);
            if (target.MergedDictionaries.Any(d => d.Source == uri))
                continue;

            target.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
        }
    }

    /// <summary>
    /// Applies the requested CopperSkin theme, resource set, chrome color, or drawing snapshot.
    /// </summary>
    public static void Apply(ResourceDictionary target, ResolvedTheme theme, CopperSkinThemeOptions? options = null)
    {
        options ??= new CopperSkinThemeOptions();
        foreach (var (key, value) in theme.Tokens)
        {
            if (IsColor(value))
                SetBrushAndColor(target, key, ToColor(value));
            else
                SetTypedValue(target, key, value);
        }

        SetGradient(target, "CopperSkin.ChromeBrush", theme.Get("color.surface.selected"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "CopperSkin.PanelBrush", theme.Get("color.surface.control"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "CopperSkin.StripBrush", theme.Get("color.surface.hover"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), false);
        SetGradient(target, "CopperSkin.ActiveBrush", theme.Get("color.accent.light"), theme.Get("color.accent.primary"), theme.Get("color.surface.selected"), true);
        SetGradient(target, "FlStudioChrome", theme.Get("color.surface.selected"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "FlStudioPanel", theme.Get("color.surface.control"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "FlStudioStrip", theme.Get("color.surface.hover"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), false);
        SetGradient(target, "FlStudioActive", theme.Get("color.accent.light"), theme.Get("color.accent.primary"), theme.Get("color.surface.selected"), true);

        var shineOpacity = ReadDouble(theme.Get("effect.shine.opacity", "0.58"), 0.58);
        target["CopperSkin.ButtonShine"] = new LinearGradientBrush(
            [
                new(Color.FromArgb((byte)(shineOpacity * 255), 255, 255, 255), 0),
                new(Color.FromArgb(0, 255, 255, 255), 0.42),
                new(Color.FromArgb(0x28, 0, 0, 0), 1)
            ],
            new Point(0, 0),
            new Point(0, 1));
        target["ButtonShine"] = target["CopperSkin.ButtonShine"];

        target["CopperSkin.PanelSheen"] = new LinearGradientBrush(
            [
                new(Color.FromArgb(0x24, 255, 255, 255), 0),
                new(Color.FromArgb(0, 0, 0, 0), 0.36),
                new(Color.FromArgb(0x24, 0, 0, 0), 1)
            ],
            new Point(0, 0),
            new Point(1, 1));
        target["PanelSheen"] = target["CopperSkin.PanelSheen"];

        var accentLight = ToColor(theme.Get("color.accent.light"));
        target["CopperSkin.SoftGlow"] = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 14, ShadowDepth = 0, Opacity = 0.48, Color = accentLight };
        target["CopperSkin.NeonGlow"] = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 20, ShadowDepth = 0, Opacity = 0.62, Color = accentLight };
        target["CopperSkin.PanelShadow"] = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 18, ShadowDepth = 1, Opacity = ReadDouble(theme.Get("effect.shadow.opacity", "0.34"), 0.34), Color = Colors.Black };
        target["SoftGlow"] = target["CopperSkin.SoftGlow"];
        target["NeonGlow"] = target["CopperSkin.NeonGlow"];
        target["PanelShadow"] = target["CopperSkin.PanelShadow"];
        SetMetricAliases(target, theme);
    }

    /// <summary>
    /// Converts a CopperSkin color token value into a WPF color.
    /// </summary>
    public static Color ToColor(string value) => (Color)ColorConverter.ConvertFromString(value);

    /// <summary>
    /// Writes both brush and color resources for a token, including legacy color aliases when needed.
    /// </summary>
    private static void SetBrushAndColor(ResourceDictionary target, string key, Color color)
    {
        target[$"{key}.color"] = color;
        target[key] = new SolidColorBrush(color);

        if (LegacyTokenAliases.Map.ContainsKey(key) || !key.StartsWith("color.", StringComparison.OrdinalIgnoreCase))
            target[$"{key}Color"] = color;
    }

    private static void SetTypedValue(ResourceDictionary target, string key, string value)
    {
        target[key] = value;
        if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var number))
        {
            target[$"{key}.double"] = number;
            if (key.StartsWith("spacing.", StringComparison.OrdinalIgnoreCase))
                target[$"{key}.thickness"] = new Thickness(number);
            if (key.StartsWith("metric.radius.", StringComparison.OrdinalIgnoreCase))
                target[$"{key}.cornerRadius"] = new CornerRadius(number);
            if (key.EndsWith(".ms", StringComparison.OrdinalIgnoreCase))
                target[$"{key}.duration"] = new Duration(TimeSpan.FromMilliseconds(number));
        }
        else if (key.StartsWith("font.", StringComparison.OrdinalIgnoreCase))
        {
            target[$"{key}.fontFamily"] = new FontFamily(value);
        }
    }

    private static void SetMetricAliases(ResourceDictionary target, ResolvedTheme theme)
    {
        target["CopperSkin.Spacing.Xs"] = new Thickness(ReadDouble(theme.Get("spacing.xs", "2"), 2));
        target["CopperSkin.Spacing.Sm"] = new Thickness(ReadDouble(theme.Get("spacing.sm", "4"), 4));
        target["CopperSkin.Spacing.Md"] = new Thickness(ReadDouble(theme.Get("spacing.md", "8"), 8));
        target["CopperSkin.Spacing.Lg"] = new Thickness(ReadDouble(theme.Get("spacing.lg", "12"), 12));
        target["CopperSkin.Spacing.Xl"] = new Thickness(ReadDouble(theme.Get("spacing.xl", "16"), 16));
        target["CopperSkin.Radius.Xs"] = new CornerRadius(ReadDouble(theme.Get("metric.radius.xs", "2"), 2));
        target["CopperSkin.Radius.Sm"] = new CornerRadius(ReadDouble(theme.Get("metric.radius.sm", "4"), 4));
        target["CopperSkin.Radius.Md"] = new CornerRadius(ReadDouble(theme.Get("metric.radius.md", "7"), 7));
        target["CopperSkin.Radius.Lg"] = new CornerRadius(ReadDouble(theme.Get("metric.radius.lg", "10"), 10));
        target["CopperSkin.Font.Ui"] = new FontFamily(theme.Get("font.ui", "Segoe UI"));
        target["CopperSkin.Font.Mono"] = new FontFamily(theme.Get("font.mono", "Consolas"));
        target["CopperSkin.Font.Size.Base"] = ReadDouble(theme.Get("font.size.base", "12"), 12);
        target["CopperSkin.Motion.Transition"] = new Duration(TimeSpan.FromMilliseconds(ReadDouble(theme.Get("motion.transition.ms", "120"), 120)));
    }

    /// <summary>
    /// Builds a three-stop themed gradient resource from resolved CopperSkin color tokens.
    /// </summary>
    private static void SetGradient(ResourceDictionary target, string key, string first, string second, string third, bool vertical) =>
        target[key] = new LinearGradientBrush(
            [
                new(ToColor(first), 0),
                new(ToColor(second), 0.54),
                new(ToColor(third), 1)
            ],
            new Point(0, 0),
            vertical ? new Point(0, 1) : new Point(1, 1));

    /// <summary>
    /// Determines whether a token value looks like a supported ARGB or RGB hex color.
    /// </summary>
    private static bool IsColor(string value) => value.StartsWith("#", StringComparison.Ordinal) && (value.Length == 7 || value.Length == 9);

    /// <summary>
    /// Reads a numeric token using invariant culture and falls back when the token is missing or invalid.
    /// </summary>
    private static double ReadDouble(string value, double fallback) =>
        double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result)
            ? result
            : fallback;
}
