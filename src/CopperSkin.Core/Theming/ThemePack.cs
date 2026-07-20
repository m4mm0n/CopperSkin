/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemePack.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 30E611E2
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
// CRC32-BODY: 30E611E2
using System.Text.Json.Serialization;
using CopperSkin.Core.Graphics;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Represents a distributable collection of CopperSkin themes.
/// </summary>
public sealed class ThemePack
{

    /// <summary>
    /// Finds a theme by id, display name, or normalized name.
    /// </summary>
    public ThemeDefinition? FindTheme(string idOrName)
    {
        return Themes.FirstOrDefault(theme =>
            string.Equals(theme.Id, idOrName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(theme.Name, idOrName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ThemeNames.Normalize(theme.Name), ThemeNames.Normalize(idOrName), StringComparison.OrdinalIgnoreCase));
    }
    /// <summary>
    /// Gets or sets the stable identifier for the theme pack.
    /// </summary>
    public string Id { get; set; } = "copperskin";

    /// <summary>
    /// Gets or sets package-level authoring, compatibility, signing, and gallery metadata.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets optional icon and basic-paint documents embedded in the pack.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GraphicDocument>? Graphics { get; set; }

    /// <summary>
    /// Gets or sets the display name for the theme pack.
    /// </summary>
    public string Name { get; set; } = "CopperSkin";

    /// <summary>
    /// Gets or sets the ordered themes distributed inside this pack.
    /// </summary>
    public List<ThemeDefinition> Themes { get; set; } = [];

    /// <summary>
    /// Gets or sets the semantic version of the theme pack.
    /// </summary>
    public string Version { get; set; } = "0.3.0.0";
}
