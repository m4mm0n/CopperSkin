/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Drawing\IThemeAwareDrawingSurface.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : 17D87BDA
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
// CRC32-BODY: 17D87BDA
namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Defines a custom drawing surface that can receive live CopperSkin drawing-theme updates.
/// </summary>
public interface IThemeAwareDrawingSurface
{
    /// <summary>
    /// Applies the supplied drawing-theme snapshot to a custom WPF drawing surface.
    /// </summary>
    void ApplyTheme(DrawingThemeSnapshot theme);
}
