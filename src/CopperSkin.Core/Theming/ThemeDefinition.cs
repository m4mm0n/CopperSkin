/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemeDefinition.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 55551E97
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
// CRC32-BODY: 55551E97
namespace CopperSkin.Core.Theming;

/// <summary>
/// Represents a single editable CopperSkin theme, including identity, inheritance, token values, and metadata.
/// </summary>
public sealed class ThemeDefinition
{
    /// <summary>
    /// Gets or sets the stable theme identifier used in packs and runtime lookup.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name shown in designer, sample, and host application theme pickers.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional parent theme id whose tokens are inherited first.
    /// </summary>
    public string? BaseThemeId { get; set; }

    /// <summary>
    /// Gets or sets the canonical and legacy-compatible token values owned by this theme.
    /// </summary>
    public Dictionary<string, string> Tokens { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets optional designer, authoring, and packaging metadata for this theme.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a deep editable copy of the current value.
    /// </summary>
    public ThemeDefinition Clone()
    {
        return new ThemeDefinition
        {
            Id = Id,
            Name = Name,
            BaseThemeId = BaseThemeId,
            Tokens = new Dictionary<string, string>(Tokens, StringComparer.OrdinalIgnoreCase),
            Metadata = new Dictionary<string, string>(Metadata, StringComparer.OrdinalIgnoreCase)
        };
    }
}
