using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf.Chrome;

public sealed class WpfWindowChromeService
{
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaBorderColor = 34;
    private const int DwmwaCaptionColor = 35;
    private const int DwmwaTextColor = 36;

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

    private static int ToColorRef(System.Windows.Media.Color color)
    {
        return color.R | (color.G << 8) | (color.B << 16);
    }

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int attributeValue, int attributeSize);
}
