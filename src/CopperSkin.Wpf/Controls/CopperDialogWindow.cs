/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\Controls\CopperDialogWindow.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : A84A6991
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
// CRC32-BODY: A84A6991
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Hosts the shared themed dialog surface used by CopperMessageBox and CopperTaskDialog.
/// </summary>
internal sealed class CopperDialogWindow : CopperWindow
{
    private readonly MessageBoxButton _buttons;

    /// <summary>
    /// Builds a compact themed dialog shell with MessageBox-compatible result handling.
    /// </summary>
    public CopperDialogWindow(string title, string message, MessageBoxButton buttons, string primaryButtonText = "OK", string secondaryButtonText = "Cancel")
    {
        _buttons = buttons;
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

        var content = new StackPanel { Margin = new Thickness(24, 20, 24, 18) };
        Grid.SetRow(content, 1);
        root.Children.Add(content);

        var title = new TextBlock
        {
            Text = Title,
            Style = TryFindResource("CopperSkin.SectionTitle") as Style,
            FontSize = 19,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 10)
        };
        content.Children.Add(title);

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
        content.Children.Add(body);

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
}
