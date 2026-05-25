/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperWindow.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:22:10 +02:00
 *  CRC32          : C6D9F17B
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
// CRC32-BODY: C6D9F17B
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Provides the base CopperSkin window with themed background, foreground, font, and chrome attachment.
/// </summary>
public class CopperWindow : Window
{
    private bool _enableWindowContextMenu = true;
    private bool _showAboutInWindowContextMenu = true;
    private bool _ownsWindowContextMenu;

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
    /// Gets or sets whether this CopperWindow provides a themed client-area context menu.
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
            maximize.IsEnabled = ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip && WindowState != WindowState.Maximized;
            about.Header = $"About {WindowTitleOrFallback()}";
            about.Visibility = _showAboutInWindowContextMenu ? Visibility.Visible : Visibility.Collapsed;
            aboutSeparator.Visibility = _showAboutInWindowContextMenu ? Visibility.Visible : Visibility.Collapsed;
        };

        return menu;
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
