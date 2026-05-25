using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Controls;
using QuickLog;

namespace CopperSkin.SampleKitchenSink;

public partial class MainWindow : CopperWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ThemeCombo.ItemsSource = BuiltInThemeCatalog.ThemeNames;
        ThemeCombo.SelectedItem = "FL Grape";
        Rows =
        [
            new ControlRow("Button", "Styled"),
            new ControlRow("TextBox", "Tokenized"),
            new ControlRow("DataGrid", "Dense"),
            new ControlRow("Window", "DWM aware")
        ];
        DataContext = this;
    }

    public IReadOnlyList<ControlRow> Rows { get; }

    private void ThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ThemeCombo.SelectedItem is not string theme || CopperSkinThemeManager.Current is null)
            return;

        CopperSkinThemeManager.Current.Apply(theme);
        StatusText.Text = $"Theme: {theme}";
        LogManager.GetDefaultLogger().Log(LogType.Info, $"Sample theme applied: {theme}");
    }

    private void Theme_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.MenuItem { Header: string theme })
            ThemeCombo.SelectedItem = theme;
    }

    private void MessageBox_Click(object sender, RoutedEventArgs e)
    {
        CopperMessageBox.Show(this, "CopperSkin owns this MessageBox-style dialog, including button styling and window chrome.", "CopperSkin MessageBox");
    }

    private void TaskDialog_Click(object sender, RoutedEventArgs e)
    {
        new CopperTaskDialog
        {
            Title = "CopperSkin TaskDialog",
            Heading = "Full theme control",
            Text = "TaskDialog-style flows use the same live resources as the rest of the engine.",
            ShowSecondaryButton = true
        }.Show(this);
    }
}

public sealed record ControlRow(string Name, string State);
