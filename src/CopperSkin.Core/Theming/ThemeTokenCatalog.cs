/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeTokenCatalog.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 2140384A
 *
 *  Description    :
 *                   Defines CopperSkin token metadata used by validation, tooling, docs, and runtime emission.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: 2140384A
// copperskin:allow-hardcoded-color-file
namespace CopperSkin.Core.Theming;

/// <summary>
/// Classifies a CopperSkin token value so validators and emitters can parse it without guessing.
/// </summary>
public enum ThemeTokenType
{
    /// <summary>A #RRGGBB or #AARRGGBB color value.</summary>
    Color,
    /// <summary>A culture-invariant numeric value.</summary>
    Number,
    /// <summary>A culture-invariant opacity value from 0.0 through 1.0.</summary>
    Opacity,
    /// <summary>A font family name.</summary>
    FontFamily,
    /// <summary>A duration in milliseconds.</summary>
    Duration,
    /// <summary>A symbolic string value.</summary>
    String
}

/// <summary>
/// Describes one known CopperSkin token, including its group, type, default value, and validation role.
/// </summary>
public sealed record ThemeTokenDefinition(
    string Key,
    ThemeTokenType Type,
    string Group,
    string Description,
    string DefaultValue,
    bool Required = false,
    bool Recommended = true);

/// <summary>
/// Central registry for CopperSkin tokens across colors, typography, spacing, radii, motion, density, and icons.
/// </summary>
public static class ThemeTokenCatalog
{

    /// <summary>
    /// Gets every known CopperSkin token definition.
    /// </summary>
    public static IReadOnlyList<ThemeTokenDefinition> All => Definitions;

    private static ThemeTokenDefinition Color(string key, string group, string description, string fallback, bool required = false) =>
        new(key, ThemeTokenType.Color, group, description, fallback, required);

    /// <summary>
    /// Gets the full recommended default token set for a professional theme pack.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Defaults => Definitions
        .Where(static d => d.Recommended)
        .ToDictionary(static d => d.Key, static d => d.DefaultValue, StringComparer.OrdinalIgnoreCase);
    private static readonly ThemeTokenDefinition[] Definitions =
    [
        Color("color.surface.deep", "Surface", "Application and window background.", "#FF101014", required: true),
        Color("color.surface.panel", "Surface", "Panel background.", "#FF1D1D24", required: true),
        Color("color.surface.control", "Surface", "Control background.", "#FF2A2A33", required: true),
        Color("color.surface.hover", "Surface", "Hover background.", "#FF343440"),
        Color("color.surface.selected", "Surface", "Selected background.", "#FF454554"),
        Color("color.accent.primary", "Accent", "Primary accent color.", "#FFB961FF", required: true),
        Color("color.accent.light", "Accent", "Light accent color.", "#FFFFA1F7", required: true),
        Color("color.border.default", "Border", "Default border color.", "#FF666675", required: true),
        Color("color.text.primary", "Text", "Primary text color.", "#FFF8F8FF", required: true),
        Color("color.text.secondary", "Text", "Secondary text color.", "#FFC9C9D8", required: true),
        Color("color.text.disabled", "Text", "Disabled text color.", "#FF777786", required: true),
        Color("color.status.success", "Status", "Success state color.", "#FF59D98E"),
        Color("color.status.warning", "Status", "Warning state color.", "#FFFFCC44"),
        Color("color.status.danger", "Status", "Danger and error state color.", "#FFFF4466"),
        Color("color.action.default", "State", "Default action surface.", "#FFB961FF"),
        Color("color.action.hover", "State", "Hovered action surface.", "#FFFFA1F7"),
        Color("color.action.pressed", "State", "Pressed action surface.", "#FF5F3F78"),
        Color("color.action.disabled", "State", "Disabled action surface.", "#FF343440"),
        Color("color.focus.ring", "State", "Keyboard focus ring.", "#FFFFA1F7"),
        Color("color.editor.playhead", "Editor", "Tracker playhead color.", "#FFFF4444"),
        Color("color.editor.grid.line", "Editor", "Tracker grid line color.", "#FF252535"),
        Color("color.editor.grid.bar", "Editor", "Tracker bar line color.", "#FF303050"),
        Color("color.editor.tracker.note", "Editor", "Tracker note cell color.", "#FFA0D0FF"),
        Color("color.editor.tracker.instrument", "Editor", "Tracker instrument cell color.", "#FFFFCC44"),
        Color("color.editor.tracker.volume", "Editor", "Tracker volume cell color.", "#FF44FF88"),
        Color("color.editor.tracker.effect", "Editor", "Tracker effect cell color.", "#FFFF8844"),
        Color("color.editor.piano.white", "Editor", "Piano roll white key color.", "#FFF4F5FF"),
        Color("color.editor.piano.black", "Editor", "Piano roll black key color.", "#FF05050B"),
        Color("color.editor.clip", "Editor", "Clip region color.", "#FFB961FF"),
        Color("color.editor.automation", "Editor", "Automation curve color.", "#FFFFA1F7"),
        Number("spacing.xs", "Spacing", "Extra-small spacing unit.", "2"),
        Number("spacing.sm", "Spacing", "Small spacing unit.", "4"),
        Number("spacing.md", "Spacing", "Medium spacing unit.", "8"),
        Number("spacing.lg", "Spacing", "Large spacing unit.", "12"),
        Number("spacing.xl", "Spacing", "Extra-large spacing unit.", "16"),
        Number("metric.radius.xs", "Radius", "Extra-small corner radius.", "2"),
        Number("metric.radius.sm", "Radius", "Small corner radius.", "4"),
        Number("metric.radius.md", "Radius", "Medium corner radius.", "7"),
        Number("metric.radius.lg", "Radius", "Large corner radius.", "10"),
        Number("metric.border.thin", "Border", "Thin border width.", "1"),
        Number("metric.border.thick", "Border", "Thick border width.", "2"),
        Number("metric.scrollbar.size", "Density", "Scrollbar thickness.", "17"),
        Number("metric.density.scale", "Density", "Global layout density multiplier.", "1"),
        Font("font.ui", "Typography", "Primary UI font family.", "Segoe UI"),
        Font("font.mono", "Typography", "Monospace font family.", "Consolas"),
        Number("font.size.sm", "Typography", "Small text size.", "11"),
        Number("font.size.base", "Typography", "Base text size.", "12"),
        Number("font.size.lg", "Typography", "Large text size.", "15"),
        Number("font.weight.normal", "Typography", "Normal font weight.", "400"),
        Number("font.weight.semibold", "Typography", "Semibold font weight.", "600"),
        Duration("motion.transition.ms", "Motion", "Default theme transition duration.", "120"),
        Duration("motion.dialog.ms", "Motion", "Dialog transition duration.", "140"),
        Opacity("effect.shine.opacity", "Effect", "Gloss highlight opacity.", "0.58"),
        Opacity("effect.shadow.opacity", "Effect", "Panel shadow opacity.", "0.34"),
        Opacity("effect.glow.opacity", "Effect", "Accent glow opacity.", "0.50"),
        String("icon.error", "Icon", "Icon key for error dialogs.", "error"),
        String("icon.warning", "Icon", "Icon key for warning dialogs.", "warning"),
        String("icon.info", "Icon", "Icon key for information dialogs.", "information"),
        String("adapter.material-design", "Adapter", "MaterialDesignInXaml adapter preference.", "auto", recommended: false),
        String("adapter.mahapps", "Adapter", "MahApps adapter preference.", "auto", recommended: false),
        String("adapter.handy-control", "Adapter", "HandyControl adapter preference.", "auto", recommended: false),
        String("adapter.avalon-dock", "Adapter", "AvalonDock adapter preference.", "auto", recommended: false)
    ];

    private static readonly IReadOnlyDictionary<string, ThemeTokenDefinition> ByKey =
        Definitions.ToDictionary(static d => d.Key, StringComparer.OrdinalIgnoreCase);

    private static ThemeTokenDefinition Duration(string key, string group, string description, string fallback) =>
        new(key, ThemeTokenType.Duration, group, description, fallback);

    private static ThemeTokenDefinition Font(string key, string group, string description, string fallback) =>
        new(key, ThemeTokenType.FontFamily, group, description, fallback);

    /// <summary>
    /// Determines whether a token key is known to CopperSkin or intentionally adapter-scoped.
    /// </summary>
    public static bool IsKnown(string key) =>
        TryGet(key, out _) ||
        key.StartsWith("metadata.", StringComparison.OrdinalIgnoreCase) ||
        key.StartsWith("adapter.", StringComparison.OrdinalIgnoreCase);

    private static ThemeTokenDefinition Number(string key, string group, string description, string fallback) =>
        new(key, ThemeTokenType.Number, group, description, fallback);

    private static ThemeTokenDefinition Opacity(string key, string group, string description, string fallback) =>
        new(key, ThemeTokenType.Opacity, group, description, fallback);

    /// <summary>
    /// Gets the token keys required for a minimally renderable CopperSkin theme.
    /// </summary>
    public static IReadOnlyList<string> RequiredKeys => Definitions.Where(static d => d.Required).Select(static d => d.Key).ToArray();

    private static ThemeTokenDefinition String(string key, string group, string description, string fallback, bool recommended = true) =>
        new(key, ThemeTokenType.String, group, description, fallback, Recommended: recommended);

    /// <summary>
    /// Looks up a token definition by key, accepting legacy aliases.
    /// </summary>
    public static bool TryGet(string key, out ThemeTokenDefinition definition)
    {
        var canonical = LegacyTokenAliases.Canonicalize(key);
        return ByKey.TryGetValue(canonical, out definition!);
    }
}
