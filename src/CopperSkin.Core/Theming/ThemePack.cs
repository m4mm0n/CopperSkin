/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemePack.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:31:37 +02:00
 *  Last Modified  : 2026-06-04 07:03:54 +02:00
 *  CRC32          : 12FE1015
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
// CRC32-BODY: 12FE1015
namespace CopperSkin.Core.Theming;

/// <summary>
/// Represents a distributable collection of CopperSkin themes.
/// </summary>
public sealed class ThemePack
{
    /// <summary>
    /// Gets or sets the stable identifier for the theme pack.
    /// </summary>
    public string Id { get; set; } = "copperskin";

    /// <summary>
    /// Gets or sets the display name for the theme pack.
    /// </summary>
    public string Name { get; set; } = "CopperSkin";

    /// <summary>
    /// Gets or sets the semantic version of the theme pack.
    /// </summary>
    public string Version { get; set; } = "0.2.0.0";

    /// <summary>
    /// Gets or sets the ordered themes distributed inside this pack.
    /// </summary>
    public List<ThemeDefinition> Themes { get; set; } = [];

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
}
