/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : samples\CopperSkin.SampleKitchenSink\MainWindow.xaml.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:39:53 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : D87E982A
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
// CRC32-BODY: D87E982A
using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Controls;
using QuickLog;

namespace CopperSkin.SampleKitchenSink;

/// <summary>
/// Hosts a CopperSkin WPF shell for live preview, editing, or sample workflows.
/// </summary>
public partial class MainWindow : CopperWindow
{
    /// <summary>
    /// Initializes the kitchen-sink sample, binds the control matrix, and selects the default CopperSkin theme.
    /// </summary>
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

    /// <summary>
    /// Gets the control rows displayed in the sample DataGrid.
    /// </summary>
    public IReadOnlyList<ControlRow> Rows { get; }

    /// <summary>
    /// Applies the selected theme from the header combo box and mirrors it into the status bar.
    /// </summary>
    private void ThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ThemeCombo.SelectedItem is not string theme || CopperSkinThemeManager.Current is null)
            return;

        CopperSkinThemeManager.Current.Apply(theme);
        StatusText.Text = $"Theme: {theme}";
        LogManager.GetDefaultLogger().Log(LogType.Info, $"Sample theme applied: {theme}");
    }

    /// <summary>
    /// Routes theme menu picks through the same combo-box path as direct selection.
    /// </summary>
    private void Theme_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.MenuItem { Header: string theme })
            ThemeCombo.SelectedItem = theme;
    }

    /// <summary>
    /// Opens the CopperSkin-owned MessageBox replacement from the sample dialog panel.
    /// </summary>
    private void MessageBox_Click(object sender, RoutedEventArgs e)
    {
        CopperMessageBox.Show(this, "CopperSkin owns this MessageBox-style dialog, including button styling and window chrome.", "CopperSkin MessageBox");
    }

    /// <summary>
    /// Opens the CopperSkin-owned TaskDialog-style surface from the sample dialog panel.
    /// </summary>
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

/// <summary>
/// Represents one control/status row displayed in the sample DataGrid.
/// </summary>
public sealed record ControlRow(string Name, string State);
