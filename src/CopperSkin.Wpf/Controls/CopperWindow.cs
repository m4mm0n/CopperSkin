/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperWindow.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:31:47 +02:00
 *  CRC32          : 95AAFE7C
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
// CRC32-BODY: 95AAFE7C
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Provides the base CopperSkin window with themed background, foreground, font, and chrome attachment.
/// </summary>
public class CopperWindow : Window
{
    private const int WmNcRButtonUp = 0x00A5;
    private const int WmSysCommand = 0x0112;
    private const int ScKeyMenu = 0xF100;
    private const int HtCaption = 2;
    private const int HtSysMenu = 3;
    private const int HtMinButton = 8;
    private const int HtMaxButton = 9;
    private const int HtClose = 20;

    private bool _enableWindowContextMenu = true;
    private bool _showAboutInWindowContextMenu = true;
    private bool _ownsWindowContextMenu;
    private HwndSource? _source;

    /// <summary>
    /// Initializes the base themed window resources and attaches native chrome refresh on load.
    /// </summary>
    public CopperWindow()
    {
        SetResourceReference(BackgroundProperty, "color.surface.deep");
        SetResourceReference(ForegroundProperty, "color.text.primary");
        FontFamily = new FontFamily("Segoe UI");
        Loaded += (_, _) =>
        {
            CopperSkinThemeManager.Current?.AttachWindow(this);
            RefreshWindowContextMenu();
        };
    }

    /// <summary>
    /// Gets or sets whether this CopperWindow provides a themed client-area and non-client window context menu.
    /// </summary>
    public bool EnableWindowContextMenu
    {
        get => _enableWindowContextMenu;
        set
        {
            _enableWindowContextMenu = value;
            RefreshWindowContextMenu();
        }
    }

    /// <summary>
    /// Gets or sets whether the themed window context menu includes an About command.
    /// </summary>
    public bool ShowAboutInWindowContextMenu
    {
        get => _showAboutInWindowContextMenu;
        set
        {
            _showAboutInWindowContextMenu = value;
            RefreshWindowContextMenu();
        }
    }

    /// <summary>
    /// Gets or sets the optional body text shown by the default About command.
    /// </summary>
    public string AboutText { get; set; } = string.Empty;

    /// <summary>
    /// Applies CopperSkin installation options that affect the base window behavior.
    /// </summary>
    internal void ApplyThemeOptions(CopperSkinThemeOptions options)
    {
        _enableWindowContextMenu = options.EnableWindowContextMenu;
        _showAboutInWindowContextMenu = options.AddAboutToWindowContextMenu;
        RefreshWindowContextMenu();
    }

    /// <summary>
    /// Opens the themed CopperSkin window context menu at a screen coordinate.
    /// </summary>
    public void ShowWindowContextMenu(Point screenPoint)
    {
        if (!_enableWindowContextMenu)
            return;

        RefreshWindowContextMenu();
        if (ContextMenu is null)
            return;

        ContextMenu.PlacementTarget = this;
        ContextMenu.Placement = PlacementMode.AbsolutePoint;
        ContextMenu.HorizontalOffset = screenPoint.X;
        ContextMenu.VerticalOffset = screenPoint.Y;
        ContextMenu.IsOpen = true;
    }

    /// <summary>
    /// Attaches the Win32 hook that replaces native title-bar context menus with the themed CopperSkin menu.
    /// </summary>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        AttachWindowHook();
    }

    /// <summary>
    /// Removes the Win32 window-message hook when the WPF window is closing.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        if (_source is not null)
        {
            _source.RemoveHook(WindowMessageHook);
            _source = null;
        }

        base.OnClosed(e);
    }

    private void RefreshWindowContextMenu()
    {
        if (!_enableWindowContextMenu)
        {
            if (_ownsWindowContextMenu)
                ContextMenu = null;
            _ownsWindowContextMenu = false;
            return;
        }

        if (ContextMenu is not null && !_ownsWindowContextMenu)
            return;

        ContextMenu = BuildWindowContextMenu();
        _ownsWindowContextMenu = true;
    }

    private ContextMenu BuildWindowContextMenu()
    {
        var restore = CreateMenuItem("Restore", () => SystemCommands.RestoreWindow(this));
        var minimize = CreateMenuItem("Minimize", () => SystemCommands.MinimizeWindow(this));
        var maximize = CreateMenuItem("Maximize", () => SystemCommands.MaximizeWindow(this));
        var close = CreateMenuItem("Close", () => SystemCommands.CloseWindow(this));
        var aboutSeparator = new Separator();
        var about = CreateMenuItem($"About {WindowTitleOrFallback()}", ShowAboutDialog);

        var menu = new ContextMenu();
        menu.Placement = PlacementMode.MousePoint;
        menu.Items.Add(restore);
        menu.Items.Add(minimize);
        menu.Items.Add(maximize);
        menu.Items.Add(new Separator());
        menu.Items.Add(close);
        menu.Items.Add(aboutSeparator);
        menu.Items.Add(about);
        menu.Opened += (_, _) =>
        {
            restore.IsEnabled = WindowState != WindowState.Normal;
            minimize.IsEnabled = ResizeMode is ResizeMode.CanMinimize or ResizeMode.CanResize or ResizeMode.CanResizeWithGrip;
            maximize.IsEnabled = (ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip) && WindowState != WindowState.Maximized;
            about.Header = $"About {WindowTitleOrFallback()}";
            about.Visibility = _showAboutInWindowContextMenu ? Visibility.Visible : Visibility.Collapsed;
            aboutSeparator.Visibility = _showAboutInWindowContextMenu ? Visibility.Visible : Visibility.Collapsed;
        };
        menu.Closed += (_, _) =>
        {
            menu.Placement = PlacementMode.MousePoint;
            menu.HorizontalOffset = 0;
            menu.VerticalOffset = 0;
        };

        return menu;
    }

    private void AttachWindowHook()
    {
        HwndSource? source = (HwndSource?)PresentationSource.FromVisual(this);
        if (source is null || ReferenceEquals(_source, source))
            return;

        _source?.RemoveHook(WindowMessageHook);
        _source = source;
        _source.AddHook(WindowMessageHook);
    }

    private nint WindowMessageHook(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        if (!_enableWindowContextMenu)
            return nint.Zero;

        if (message == WmNcRButtonUp && IsWindowMenuHitTest(wParam))
        {
            ShowWindowContextMenu(ScreenPointFromLParam(lParam));
            handled = true;
            return nint.Zero;
        }

        if (message == WmSysCommand && IsKeyMenuCommand(wParam))
        {
            ShowWindowContextMenu(DefaultSystemMenuPoint());
            handled = true;
        }

        return nint.Zero;
    }

    private static bool IsWindowMenuHitTest(nint hitTest)
    {
        int code = hitTest.ToInt32();
        return code is HtCaption or HtSysMenu or HtMinButton or HtMaxButton or HtClose;
    }

    private static bool IsKeyMenuCommand(nint command)
    {
        return (command.ToInt32() & 0xFFF0) == ScKeyMenu;
    }

    private Point ScreenPointFromLParam(nint lParam)
    {
        var devicePoint = new Point(SignedLowWord(lParam), SignedHighWord(lParam));
        PresentationSource? source = PresentationSource.FromVisual(this);
        return source?.CompositionTarget?.TransformFromDevice.Transform(devicePoint) ?? devicePoint;
    }

    private static int SignedLowWord(nint value)
    {
        return unchecked((short)(value.ToInt64() & 0xFFFF));
    }

    private static int SignedHighWord(nint value)
    {
        return unchecked((short)((value.ToInt64() >> 16) & 0xFFFF));
    }

    private Point DefaultSystemMenuPoint()
    {
        return new Point(Left + 12, Top + 32);
    }

    private static MenuItem CreateMenuItem(string header, Action action)
    {
        var item = new MenuItem { Header = header };
        item.Click += (_, _) => action();
        return item;
    }

    private string WindowTitleOrFallback()
    {
        return string.IsNullOrWhiteSpace(Title) ? "CopperSkin" : Title;
    }

    private void ShowAboutDialog()
    {
        string title = WindowTitleOrFallback();
        string message = string.IsNullOrWhiteSpace(AboutText)
            ? $"{title}\nCopperSkin WPF Theme Engine"
            : AboutText;
        CopperMessageBox.Show(this, message, $"About {title}");
    }
}
