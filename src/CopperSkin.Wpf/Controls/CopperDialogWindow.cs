using System.Windows;
using System.Windows.Controls;

namespace CopperSkin.Wpf.Controls;

internal sealed class CopperDialogWindow : CopperWindow
{
    private readonly MessageBoxButton _buttons;

    public CopperDialogWindow(string title, string message, MessageBoxButton buttons)
    {
        _buttons = buttons;
        Title = title;
        Width = 460;
        MinHeight = 210;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;
        PrimaryButtonText = "OK";
        SecondaryButtonText = "Cancel";
        Content = Build(message);
    }

    public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;

    public string PrimaryButtonText { get; set; }

    public string SecondaryButtonText { get; set; }

    private UIElement Build(string message)
    {
        var root = new Grid { Margin = new Thickness(18) };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var title = new TextBlock
        {
            Text = Title,
            Style = TryFindResource("CopperSkin.SectionTitle") as Style,
            Margin = new Thickness(0, 0, 0, 10)
        };
        Grid.SetRow(title, 0);
        root.Children.Add(title);

        var body = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 18)
        };
        Grid.SetRow(body, 1);
        root.Children.Add(body);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        var ok = new Button { Content = PrimaryButtonText, MinWidth = 92, Margin = new Thickness(8, 0, 0, 0), IsDefault = true };
        ok.Click += (_, _) => { Result = MessageBoxResult.OK; DialogResult = true; };
        buttons.Children.Add(ok);

        if (_buttons is MessageBoxButton.OKCancel or MessageBoxButton.YesNo or MessageBoxButton.YesNoCancel)
        {
            var cancel = new Button { Content = SecondaryButtonText, MinWidth = 92, Margin = new Thickness(8, 0, 0, 0), IsCancel = true };
            cancel.Click += (_, _) => { Result = MessageBoxResult.Cancel; DialogResult = false; };
            buttons.Children.Add(cancel);
        }

        Grid.SetRow(buttons, 2);
        root.Children.Add(buttons);
        return root;
    }
}
