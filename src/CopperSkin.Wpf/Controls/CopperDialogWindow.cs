/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperDialogWindow.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-06-04 07:03:54 +02:00
 *  CRC32          : FCC85ADC
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
// CRC32-BODY: FCC85ADC
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Hosts the shared themed dialog surface used by CopperMessageBox and CopperTaskDialog.
/// </summary>
internal sealed class CopperDialogWindow : CopperWindow
{
    private readonly MessageBoxButton _buttons;
    private readonly CopperTaskDialogIcon _icon;

    /// <summary>
    /// Builds a compact themed dialog shell with MessageBox-compatible result handling.
    /// </summary>
    public CopperDialogWindow(
        string title,
        string message,
        MessageBoxButton buttons,
        string primaryButtonText = "OK",
        string secondaryButtonText = "Cancel",
        CopperTaskDialogIcon icon = CopperTaskDialogIcon.None)
    {
        _buttons = buttons;
        _icon = icon;
        Title = title;
        Width = 520;
        SizeToContent = SizeToContent.Height;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;
        PrimaryButtonText = primaryButtonText;
        SecondaryButtonText = secondaryButtonText;
        SetResourceReference(BackgroundProperty, "color.surface.deep");
        SetResourceReference(ForegroundProperty, "color.text.primary");
        Content = Build(message);
    }

    /// <summary>
    /// Gets the MessageBox-compatible result selected by the user.
    /// </summary>
    public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;

    /// <summary>
    /// Gets or sets the caption displayed on the primary dialog button.
    /// </summary>
    public string PrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the caption displayed on the secondary dialog button.
    /// </summary>
    public string SecondaryButtonText { get; set; }

    /// <summary>
    /// Builds the full themed WPF visual tree for the component.
    /// </summary>
    private UIElement Build(string message)
    {
        var shell = new Border
        {
            Margin = new Thickness(14),
            Padding = new Thickness(0),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6)
        };
        shell.SetResourceReference(Border.BackgroundProperty, "color.surface.panel");
        shell.SetResourceReference(Border.BorderBrushProperty, "color.border.default");

        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        shell.Child = root;

        var accent = new Border { CornerRadius = new CornerRadius(6, 6, 0, 0) };
        accent.SetResourceReference(Border.BackgroundProperty, "CopperSkin.ActiveBrush");
        Grid.SetRow(accent, 0);
        root.Children.Add(accent);

        var content = new Grid { Margin = new Thickness(24, 20, 24, 18) };
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(content, 1);
        root.Children.Add(content);

        var textColumn = new StackPanel();
        if (_icon is not CopperTaskDialogIcon.None && Win32StockIconSource.TryCreate(_icon) is { } source)
        {
            var iconHost = new Border
            {
                Width = 46,
                Height = 46,
                Margin = new Thickness(0, 1, 16, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                CornerRadius = new CornerRadius(6)
            };
            iconHost.SetResourceReference(Border.BackgroundProperty, "color.surface.control");
            iconHost.Child = new Image
            {
                Source = source,
                Width = 32,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            content.Children.Add(iconHost);
            Grid.SetColumn(textColumn, 1);
        }
        else
        {
            Grid.SetColumnSpan(textColumn, 2);
        }
        content.Children.Add(textColumn);

        var title = new TextBlock
        {
            Text = Title,
            Style = TryFindResource("CopperSkin.SectionTitle") as Style,
            FontSize = 19,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 10)
        };
        textColumn.Children.Add(title);

        var body = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13,
            LineHeight = 19,
            MaxWidth = 448,
            Margin = new Thickness(0, 0, 0, 2)
        };
        body.SetResourceReference(TextBlock.ForegroundProperty, "color.text.secondary");
        textColumn.Children.Add(body);

        var tray = new Border
        {
            Padding = new Thickness(16, 12, 16, 16),
            BorderThickness = new Thickness(0, 1, 0, 0)
        };
        tray.SetResourceReference(Border.BackgroundProperty, "color.surface.control");
        tray.SetResourceReference(Border.BorderBrushProperty, "color.border.default");

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        tray.Child = buttons;
        var ok = new Button { Content = PrimaryButtonText, MinWidth = 108, MinHeight = 32, Margin = new Thickness(8, 0, 0, 0), IsDefault = true };
        ok.Click += (_, _) => { Result = MessageBoxResult.OK; DialogResult = true; };
        buttons.Children.Add(ok);

        if (_buttons is MessageBoxButton.OKCancel or MessageBoxButton.YesNo or MessageBoxButton.YesNoCancel)
        {
            var cancel = new Button { Content = SecondaryButtonText, MinWidth = 108, MinHeight = 32, Margin = new Thickness(8, 0, 0, 0), IsCancel = true };
            cancel.Click += (_, _) => { Result = MessageBoxResult.Cancel; DialogResult = false; };
            buttons.Children.Add(cancel);
        }

        Grid.SetRow(tray, 2);
        root.Children.Add(tray);
        return shell;
    }

    private static class Win32StockIconSource
    {
        private const int IdiApplication = 32512;
        private const int IdiError = 32513;
        private const int IdiQuestion = 32514;
        private const int IdiWarning = 32515;
        private const int IdiInformation = 32516;
        private const int IdiShield = 32518;

        public static ImageSource? TryCreate(CopperTaskDialogIcon icon)
        {
            var handle = LoadIcon(IntPtr.Zero, new IntPtr(ToResourceId(icon)));
            return handle == IntPtr.Zero
                ? null
                : Imaging.CreateBitmapSourceFromHIcon(handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(32, 32));
        }

        private static int ToResourceId(CopperTaskDialogIcon icon) => icon switch
        {
            CopperTaskDialogIcon.Application => IdiApplication,
            CopperTaskDialogIcon.Information => IdiInformation,
            CopperTaskDialogIcon.Warning => IdiWarning,
            CopperTaskDialogIcon.Error => IdiError,
            CopperTaskDialogIcon.Question => IdiQuestion,
            CopperTaskDialogIcon.Shield => IdiShield,
            _ => IdiInformation
        };

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);
    }
}
