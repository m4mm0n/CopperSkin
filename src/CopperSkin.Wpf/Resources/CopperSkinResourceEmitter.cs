// copperskin:allow-hardcoded-color-file
using System.Windows;
using System.Windows.Media;
using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf.Resources;

public static class CopperSkinResourceEmitter
{
    private static readonly string[] DictionarySources =
    [
        "/CopperSkin.Wpf;component/Themes/CopperSkin.Controls.xaml"
    ];

    public static void MergeDefaultDictionaries(ResourceDictionary target)
    {
        foreach (string source in DictionarySources)
        {
            var uri = new Uri(source, UriKind.Relative);
            if (target.MergedDictionaries.Any(d => d.Source == uri))
                continue;

            target.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
        }
    }

    public static void Apply(ResourceDictionary target, ResolvedTheme theme, CopperSkinThemeOptions? options = null)
    {
        options ??= new CopperSkinThemeOptions();
        foreach (var (key, value) in theme.Tokens)
        {
            if (IsColor(value))
                SetBrushAndColor(target, key, ToColor(value));
            else
                target[key] = value;
        }

        SetGradient(target, "CopperSkin.ChromeBrush", theme.Get("color.surface.selected"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "CopperSkin.PanelBrush", theme.Get("color.surface.control"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "CopperSkin.StripBrush", theme.Get("color.surface.hover"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), false);
        SetGradient(target, "CopperSkin.ActiveBrush", theme.Get("color.accent.light"), theme.Get("color.accent.primary"), theme.Get("color.surface.selected"), true);
        SetGradient(target, "FlStudioChrome", theme.Get("color.surface.selected"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "FlStudioPanel", theme.Get("color.surface.control"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), true);
        SetGradient(target, "FlStudioStrip", theme.Get("color.surface.hover"), theme.Get("color.surface.panel"), theme.Get("color.surface.deep"), false);
        SetGradient(target, "FlStudioActive", theme.Get("color.accent.light"), theme.Get("color.accent.primary"), theme.Get("color.surface.selected"), true);

        double shineOpacity = ReadDouble(theme.Get("effect.shine.opacity", "0.58"), 0.58);
        target["CopperSkin.ButtonShine"] = new LinearGradientBrush(
            new GradientStopCollection
            {
                new(Color.FromArgb((byte)(shineOpacity * 255), 255, 255, 255), 0),
                new(Color.FromArgb(0, 255, 255, 255), 0.42),
                new(Color.FromArgb(0x28, 0, 0, 0), 1)
            },
            new Point(0, 0),
            new Point(0, 1));
        target["ButtonShine"] = target["CopperSkin.ButtonShine"];

        target["CopperSkin.PanelSheen"] = new LinearGradientBrush(
            new GradientStopCollection
            {
                new(Color.FromArgb(0x24, 255, 255, 255), 0),
                new(Color.FromArgb(0, 0, 0, 0), 0.36),
                new(Color.FromArgb(0x24, 0, 0, 0), 1)
            },
            new Point(0, 0),
            new Point(1, 1));
        target["PanelSheen"] = target["CopperSkin.PanelSheen"];

        Color accentLight = ToColor(theme.Get("color.accent.light"));
        target["CopperSkin.SoftGlow"] = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 14, ShadowDepth = 0, Opacity = 0.48, Color = accentLight };
        target["CopperSkin.NeonGlow"] = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 20, ShadowDepth = 0, Opacity = 0.62, Color = accentLight };
        target["CopperSkin.PanelShadow"] = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 18, ShadowDepth = 1, Opacity = ReadDouble(theme.Get("effect.shadow.opacity", "0.34"), 0.34), Color = Colors.Black };
        target["SoftGlow"] = target["CopperSkin.SoftGlow"];
        target["NeonGlow"] = target["CopperSkin.NeonGlow"];
        target["PanelShadow"] = target["CopperSkin.PanelShadow"];
    }

    public static Color ToColor(string value)
    {
        return (Color)ColorConverter.ConvertFromString(value);
    }

    private static void SetBrushAndColor(ResourceDictionary target, string key, Color color)
    {
        target[$"{key}.color"] = color;
        target[key] = new SolidColorBrush(color);

        if (LegacyTokenAliases.Map.ContainsKey(key) || !key.StartsWith("color.", StringComparison.OrdinalIgnoreCase))
            target[$"{key}Color"] = color;
    }

    private static void SetGradient(ResourceDictionary target, string key, string first, string second, string third, bool vertical)
    {
        target[key] = new LinearGradientBrush(
            new GradientStopCollection
            {
                new(ToColor(first), 0),
                new(ToColor(second), 0.54),
                new(ToColor(third), 1)
            },
            new Point(0, 0),
            vertical ? new Point(0, 1) : new Point(1, 1));
    }

    private static bool IsColor(string value)
    {
        return value.StartsWith("#", StringComparison.Ordinal) && (value.Length == 7 || value.Length == 9);
    }

    private static double ReadDouble(string value, double fallback)
    {
        return double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result)
            ? result
            : fallback;
    }
}
