/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinThemeOptions.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : 82D4C894
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
// CRC32-BODY: 82D4C894
namespace CopperSkin.Wpf;

/// <summary>
/// Configures how CopperSkin installs resources, styles, and native window chrome.
/// </summary>
public sealed class CopperSkinThemeOptions
{
    /// <summary>
    /// Gets or sets whether legacy amChipper token aliases are emitted for compatibility.
    /// </summary>
    public bool IncludeLegacyAliases { get; set; } = true;

    /// <summary>
    /// Gets or sets whether supported native title-bar chrome is tinted from the active theme.
    /// </summary>
    public bool ApplyNativeWindowChrome { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the bundled CopperSkin standard-control templates are merged.
    /// </summary>
    public bool MergeDefaultControlStyles { get; set; } = true;
}
