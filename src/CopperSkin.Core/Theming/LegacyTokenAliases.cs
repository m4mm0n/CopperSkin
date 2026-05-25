namespace CopperSkin.Core.Theming;

public static class LegacyTokenAliases
{
    public static readonly IReadOnlyDictionary<string, string> Map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["BgDeep"] = "color.surface.deep",
        ["BgPanel"] = "color.surface.panel",
        ["BgControl"] = "color.surface.control",
        ["BgHover"] = "color.surface.hover",
        ["BgSelect"] = "color.surface.selected",
        ["Accent"] = "color.accent.primary",
        ["AccentLight"] = "color.accent.light",
        ["Border"] = "color.border.default",
        ["TextPrimary"] = "color.text.primary",
        ["TextSecondary"] = "color.text.secondary",
        ["TextDisabled"] = "color.text.disabled",
        ["Playhead"] = "color.editor.playhead",
        ["GridLine"] = "color.editor.grid.line",
        ["GridBar"] = "color.editor.grid.bar",
        ["FlStudioChrome"] = "brush.chrome.main",
        ["FlStudioPanel"] = "brush.panel.main",
        ["FlStudioStrip"] = "brush.panel.strip",
        ["FlStudioActive"] = "brush.accent.active",
        ["ButtonShine"] = "effect.shine.button",
        ["PanelSheen"] = "effect.shine.panel",
        ["SoftGlow"] = "effect.glow.soft",
        ["PanelShadow"] = "effect.shadow.panel",
        ["NeonGlow"] = "effect.glow.neon"
    };

    public static string Canonicalize(string key)
    {
        return Map.TryGetValue(key, out string? canonical) ? canonical : key;
    }
}
