/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperWindow.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : BC8C505F
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
// CRC32-BODY: BC8C505F
using System.Windows;
using System.Windows.Media;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Provides the base CopperSkin window with themed background, foreground, font, and chrome attachment.
/// </summary>
public class CopperWindow : Window
{
    /// <summary>
    /// Initializes the base themed window resources and attaches native chrome refresh on load.
    /// </summary>
    public CopperWindow()
    {
        SetResourceReference(BackgroundProperty, "color.surface.deep");
        SetResourceReference(ForegroundProperty, "color.text.primary");
        FontFamily = new FontFamily("Segoe UI");
        Loaded += (_, _) => CopperSkinThemeManager.Current?.AttachWindow(this);
    }
}
