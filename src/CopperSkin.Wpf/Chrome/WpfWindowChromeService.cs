/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Chrome\WpfWindowChromeService.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : F0C808F2
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
// CRC32-BODY: F0C808F2
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf.Chrome;

/// <summary>
/// Applies CopperSkin accent colors to supported native WPF window chrome.
/// </summary>
public sealed class WpfWindowChromeService
{

    /// <summary>
    /// Applies the requested CopperSkin theme, resource set, chrome color, or drawing snapshot.
    /// </summary>
    public void Apply(Window window, ResolvedTheme theme, CopperSkinThemeOptions? options = null)
    {
        if (!OperatingSystem.IsWindows())
            return;

        options ??= new CopperSkinThemeOptions();
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == nint.Zero)
            return;

        var darkMode = 1;
        _ = DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkMode, ref darkMode, sizeof(int));

        var caption = ToColorRef(CopperSkinResourceEmitter.ToColor(theme.Get("chrome.caption.background", theme.Get("color.surface.deep"))));
        var border = ToColorRef(CopperSkinResourceEmitter.ToColor(theme.Get("chrome.caption.border", theme.Get("color.accent.primary"))));
        var text = ToColorRef(CopperSkinResourceEmitter.ToColor(theme.Get("chrome.caption.foreground", theme.Get("color.text.primary"))));

        _ = DwmSetWindowAttribute(handle, DwmwaCaptionColor, ref caption, sizeof(int));
        _ = DwmSetWindowAttribute(handle, DwmwaBorderColor, ref border, sizeof(int));
        _ = DwmSetWindowAttribute(handle, DwmwaTextColor, ref text, sizeof(int));

        var backdrop = ToBackdropValue(options.BackdropKind);
        if (backdrop >= 0)
            _ = DwmSetWindowAttribute(handle, DwmwaSystemBackdropType, ref backdrop, sizeof(int));
    }

    /// <summary>
    /// Calls the Windows DWM API used to color title bars, borders, and caption text.
    /// </summary>
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int attributeValue, int attributeSize);
    private const int DwmwaBorderColor = 34;
    private const int DwmwaCaptionColor = 35;
    private const int DwmwaSystemBackdropType = 38;
    private const int DwmwaTextColor = 36;
    private const int DwmwaUseImmersiveDarkMode = 20;

    private static int ToBackdropValue(CopperSkinBackdropKind backdropKind) => backdropKind switch
    {
        CopperSkinBackdropKind.None => 1,
        CopperSkinBackdropKind.Auto => 0,
        CopperSkinBackdropKind.Mica => 2,
        CopperSkinBackdropKind.Acrylic => 3,
        CopperSkinBackdropKind.Tabbed => 4,
        _ => -1
    };

    /// <summary>
    /// Converts a WPF color into the COLORREF integer layout required by DWM.
    /// </summary>
    private static int ToColorRef(System.Windows.Media.Color color) => color.R | (color.G << 8) | (color.B << 16);
}
