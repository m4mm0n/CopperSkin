/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinThemeOptions.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : F8760DF1
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
// CRC32-BODY: F8760DF1
namespace CopperSkin.Wpf;

/// <summary>
/// Selects the preferred Windows 11 system backdrop for CopperSkin windows.
/// </summary>
public enum CopperSkinBackdropKind
{
    /// <summary>Do not request a system backdrop.</summary>
    None,
    /// <summary>Let Windows choose the most appropriate backdrop.</summary>
    Auto,
    /// <summary>Request Mica when supported by the operating system.</summary>
    Mica,
    /// <summary>Request Acrylic when supported by the operating system.</summary>
    Acrylic,
    /// <summary>Request tabbed Mica when supported by the operating system.</summary>
    Tabbed
}

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
    /// Gets or sets the preferred Windows 11 backdrop for windows attached to CopperSkin.
    /// </summary>
    public CopperSkinBackdropKind BackdropKind { get; set; } = CopperSkinBackdropKind.Auto;

    /// <summary>
    /// Gets or sets whether the bundled CopperSkin standard-control templates are merged.
    /// </summary>
    public bool MergeDefaultControlStyles { get; set; } = true;

    /// <summary>
    /// Gets or sets whether CopperWindow installs themed client-area and title-bar window context menus.
    /// </summary>
    public bool EnableWindowContextMenu { get; set; } = true;

    /// <summary>
    /// Gets or sets whether CopperWindow adds an About command to its themed window context menu.
    /// </summary>
    public bool AddAboutToWindowContextMenu { get; set; } = true;

    /// <summary>
    /// Gets or sets the initial theme applied when CopperSkin is installed.
    /// </summary>
    public string DefaultThemeName { get; set; } = "FL Grape";
}
