/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\LegacyTokenAliases.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 1B900978
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
// CRC32-BODY: 1B900978
namespace CopperSkin.Core.Theming;

/// <summary>
/// Maps amChipper-era token names onto the canonical CopperSkin token schema.
/// </summary>
public static class LegacyTokenAliases
{
    /// <summary>
    /// Documents this CopperSkin member.
    /// </summary>
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

    /// <summary>
    /// Returns the canonical token key for a legacy or modern token name.
    /// </summary>
    public static string Canonicalize(string key)
    {
        return Map.TryGetValue(key, out string? canonical) ? canonical : key;
    }
}
