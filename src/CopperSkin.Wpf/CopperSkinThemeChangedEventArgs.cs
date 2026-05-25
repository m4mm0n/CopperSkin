/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinThemeChangedEventArgs.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:07:57 +02:00
 *  CRC32          : 8ABB8991
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
// CRC32-BODY: 8ABB8991
using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf;

/// <summary>
/// Provides event data when the active CopperSkin theme changes.
/// </summary>
public sealed class CopperSkinThemeChangedEventArgs : EventArgs
{
    /// <summary>
    /// Creates theme-change event data for the resolved theme that was just applied.
    /// </summary>
    public CopperSkinThemeChangedEventArgs(ResolvedTheme theme)
    {
        Theme = theme;
    }

    /// <summary>
    /// Gets the resolved theme that has just been applied.
    /// </summary>
    public ResolvedTheme Theme { get; }
}
