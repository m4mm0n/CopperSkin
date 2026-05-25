/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Chrome\WpfWindowChromeService.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : ED8885ED
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
// CRC32-BODY: ED8885ED
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
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaBorderColor = 34;
    private const int DwmwaCaptionColor = 35;
    private const int DwmwaTextColor = 36;

    /// <summary>
    /// Applies the requested CopperSkin theme, resource set, chrome color, or drawing snapshot.
    /// </summary>
    public void Apply(Window window, ResolvedTheme theme)
    {
        if (!OperatingSystem.IsWindows())
            return;

        nint handle = new WindowInteropHelper(window).Handle;
        if (handle == nint.Zero)
            return;

        int darkMode = 1;
        _ = DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkMode, ref darkMode, sizeof(int));

        int caption = ToColorRef(CopperSkinResourceEmitter.ToColor(theme.Get("chrome.caption.background", theme.Get("color.surface.deep"))));
        int border = ToColorRef(CopperSkinResourceEmitter.ToColor(theme.Get("chrome.caption.border", theme.Get("color.accent.primary"))));
        int text = ToColorRef(CopperSkinResourceEmitter.ToColor(theme.Get("chrome.caption.foreground", theme.Get("color.text.primary"))));

        _ = DwmSetWindowAttribute(handle, DwmwaCaptionColor, ref caption, sizeof(int));
        _ = DwmSetWindowAttribute(handle, DwmwaBorderColor, ref border, sizeof(int));
        _ = DwmSetWindowAttribute(handle, DwmwaTextColor, ref text, sizeof(int));
    }

    /// <summary>
    /// Converts a WPF color into the COLORREF integer layout required by DWM.
    /// </summary>
    private static int ToColorRef(System.Windows.Media.Color color)
    {
        return color.R | (color.G << 8) | (color.B << 16);
    }

    /// <summary>
    /// Calls the Windows DWM API used to color title bars, borders, and caption text.
    /// </summary>
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int attributeValue, int attributeSize);
}
