/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Designer\MainWindow.xaml.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:39:02 +02:00
 *  Last Modified  : 2026-07-03 09:57:50 +02:00
 *  CRC32          : 51DF4D52
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
// CRC32-BODY: 51DF4D52

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Controls;
using Microsoft.Win32;
using QuickLog;

namespace CopperSkin.Designer;

/// <summary>
/// Hosts a CopperSkin WPF shell for live preview, editing, or sample workflows.
/// </summary>
public partial class MainWindow : CopperWindow, INotifyPropertyChanged
{
    /// <summary>
    /// Holds the built-in catalog that the designer clones into editable preview themes.
    /// </summary>
    private readonly ThemePack _runtimePack = BuiltInThemeCatalog.Create();
    private ThemeDefinition? _selectedTheme;
    private TokenRow? _selectedToken;
    private string? _lastLoggedThemeId;

    /// <summary>
    /// Initializes the designer window, loads editable themes, and selects the first preview theme.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Themes = new ObservableCollection<ThemeDefinition>(_runtimePack.Themes.Select(static t => t.Clone()));
        Diagnostics = [];
        EventLog = [];
        PreviewRows =
        [
            new("ButtonBase", "Normal/Hover/Pressed", "Button, RepeatButton, ToggleButton"),
            new("TextBoxBase", "Caret/Selection", "TextBox and RichTextBox"),
            new("ItemsControl", "Selected/Hover", "ListBox, ListView, TreeView"),
            new("DataGrid", "Rows/Cells/Headers", "Data grid rows, cells, headers, details"),
            new("Documents", "Readable", "FlowDocument, Paragraph, List, Hyperlink"),
            new("Chrome", "Native/Themed", "Menu, ContextMenu, ToolBar, StatusBar")
        ];
        Tokens = [];
        TokenCatalog = new ObservableCollection<TokenCatalogRow>(
            ThemeTokenCatalog.All
                .OrderBy(static t => t.Group, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static t => t.Key, StringComparer.OrdinalIgnoreCase)
                .Select(static t => new TokenCatalogRow(t)));
        DataContext = this;
        ThemeList.SelectedIndex = 0;
        Log(LogType.Info, "Designer window initialized");
    }

    /// <summary>
    /// Raised when designer bindable state changes and the WPF view needs to refresh.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the editable theme catalog displayed by the designer.
    /// </summary>
    public ObservableCollection<ThemeDefinition> Themes { get; }

    /// <summary>
    /// Gets the token rows for the currently selected theme.
    /// </summary>
    public ObservableCollection<TokenRow> Tokens { get; }

    /// <summary>
    /// Gets representative standard WPF rows shown by the preview tab.
    /// </summary>
    public ObservableCollection<PreviewRow> PreviewRows { get; }

    /// <summary>
    /// Gets the canonical token catalog shown inside the designer.
    /// </summary>
    public ObservableCollection<TokenCatalogRow> TokenCatalog { get; }

    /// <summary>
    /// Gets the validation diagnostics shown in the designer.
    /// </summary>
    public ObservableCollection<string> Diagnostics { get; }

    /// <summary>
    /// Gets the recent Designer event log shown in the shell.
    /// </summary>
    public ObservableCollection<string> EventLog { get; }

    /// <summary>
    /// Gets or sets the token row currently selected in the live token grid.
    /// </summary>
    public TokenRow? SelectedToken
    {
        get => _selectedToken;
        set { _selectedToken = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Gets the designer status text displayed in the status bar.
    /// </summary>
    public string StatusText { get; private set; } = "Ready";

    /// <summary>
    /// Loads tokens for the selected theme and refreshes the live preview.
    /// </summary>
    private void ThemeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        _selectedTheme = ThemeList.SelectedItem as ThemeDefinition;
        LoadSelectedTheme();
    }

    /// <summary>
    /// Rebuilds the token grid from the selected theme and applies it to the preview resources.
    /// </summary>
    private void LoadSelectedTheme()
    {
        if (_selectedTheme is null)
            return;

        Tokens.Clear();
        foreach (var pair in _selectedTheme.Tokens.OrderBy(static t => t.Key, StringComparer.OrdinalIgnoreCase))
            Tokens.Add(new TokenRow(pair.Key, pair.Value));

        if (!string.Equals(_lastLoggedThemeId, _selectedTheme.Id, StringComparison.OrdinalIgnoreCase))
        {
            Log(LogType.Info, $"Selected theme {_selectedTheme.Name}");
            _lastLoggedThemeId = _selectedTheme.Id;
        }

        ApplyCurrentTheme();
    }

    /// <summary>
    /// Applies the currently edited token value when the user clicks the apply button.
    /// </summary>
    private void ApplyToken_Click(object sender, RoutedEventArgs e) => ApplyTokenEdit(logEdit: true);

    /// <summary>
    /// Live-applies token edits as the value text box changes.
    /// </summary>
    private void TokenValueBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ApplyTokenEdit(logEdit: false);

    /// <summary>
    /// Writes the selected token value into the editable theme and refreshes the preview.
    /// </summary>
    private void ApplyTokenEdit(bool logEdit)
    {
        if (_selectedTheme is null || SelectedToken is null)
            return;

        _selectedTheme.Tokens[SelectedToken.Key] = SelectedToken.Value;
        if (logEdit)
            Log(LogType.Info, $"Applied token {SelectedToken.Key}");
        ApplyCurrentTheme();
    }

    /// <summary>
    /// Validates the edited theme, applies its resources, updates drawing surfaces, and refreshes status text.
    /// </summary>
    private void ApplyCurrentTheme()
    {
        if (_selectedTheme is null)
            return;

        var tempPack = new ThemePack { Id = "designer.preview", Name = "Designer Preview", Themes = [_selectedTheme.Clone()] };
        var diagnostics = new ThemeValidator().Validate(tempPack);
        Diagnostics.Clear();
        foreach (var diagnostic in diagnostics)
            Diagnostics.Add(diagnostic.ToString());

        var resolved = new ThemeResolver().Resolve(tempPack, _selectedTheme.Id);
        Wpf.Resources.CopperSkinResourceEmitter.Apply(Application.Current.Resources, resolved);
        Wpf.Drawing.DrawingThemeRegistry.Apply(Wpf.Drawing.DrawingThemeSnapshot.FromTheme(resolved));
        StatusText = $"Previewing {_selectedTheme.Name} - {diagnostics.Count} diagnostics";
        OnPropertyChanged(nameof(StatusText));
    }

    /// <summary>
    /// Clones the selected theme into a new editable custom theme.
    /// </summary>
    private void DuplicateTheme_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedTheme is null)
            return;

        var copy = _selectedTheme.Clone();
        copy.Name = $"{copy.Name} Custom";
        copy.Id = ThemeNames.Slug(copy.Name);
        Themes.Add(copy);
        ThemeList.SelectedItem = copy;
        Log(LogType.Info, $"Duplicated theme {copy.Name}");
    }

    /// <summary>
    /// Validates the entire editable pack and lists all diagnostics in the designer.
    /// </summary>
    private void ValidatePack_Click(object sender, RoutedEventArgs e)
    {
        var pack = new ThemePack { Id = "designer.current", Name = "Designer Current" };
        pack.Themes.AddRange(Themes.Select(static t => t.Clone()));
        Diagnostics.Clear();
        foreach (var diagnostic in new ThemeValidator().Validate(pack))
            Diagnostics.Add(diagnostic.ToString());
        StatusText = $"Validated {pack.Themes.Count} themes";
        OnPropertyChanged(nameof(StatusText));
        Log(LogType.Info, $"Validated {pack.Themes.Count} themes with {Diagnostics.Count} diagnostics");
    }

    /// <summary>
    /// Exports the current editable theme pack to a CopperSkin JSON theme-pack file.
    /// </summary>
    private void ExportThemePack_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CopperSkin theme pack (*.json)|*.json",
            FileName = "copperskin-theme-pack.json"
        };
        if (dialog.ShowDialog(this) != true)
            return;

        var pack = new ThemePack { Id = "designer.export", Name = "CopperSkin Designer Export" };
        pack.Themes.AddRange(Themes.Select(static t => t.Clone()));
        ThemeJsonSerializer.WritePack(dialog.FileName, pack);
        StatusText = $"Exported {dialog.FileName}";
        OnPropertyChanged(nameof(StatusText));
        Log(LogType.Info, $"Exported theme pack to {dialog.FileName}");
    }

    /// <summary>
    /// Opens the themed TaskDialog preview from the designer command surface.
    /// </summary>
    private void TaskDialog_Click(object sender, RoutedEventArgs e)
    {
        Log(LogType.Info, "Opened TaskDialog preview");
        new CopperTaskDialog
        {
            Title = "CopperSkin TaskDialog",
            Heading = "Live themed dialog",
            Text = "This is a CopperSkin-controlled TaskDialog-style surface using the active theme.",
            ShowSecondaryButton = true
        }.Show(this);
    }

    /// <summary>
    /// Raises a property-change notification for WPF binding refresh.
    /// </summary>
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void Log(LogType type, string message, Exception? exception = null)
    {
        var logger = LogManager.GetDefaultLogger();
        if (exception is null)
            logger?.Log(type, message);
        else
            logger?.Log(type, message, exception);

        EventLog.Insert(0, $"{DateTimeOffset.Now:HH:mm:ss} [{type}] {message}");
        while (EventLog.Count > 200)
            EventLog.RemoveAt(EventLog.Count - 1);
    }
}

/// <summary>
/// Represents one standard WPF preview row in the Designer preview tab.
/// </summary>
public sealed record PreviewRow(string Component, string State, string Notes);

/// <summary>
/// Represents one editable token row in the designer token grid.
/// </summary>
public sealed class TokenRow : INotifyPropertyChanged
{
    private string _value;

    /// <summary>
    /// Creates an editable token row with a stable token key and initial value.
    /// </summary>
    public TokenRow(string key, string value)
    {
        Key = key;
        _value = value;
    }

    /// <summary>
    /// Raised when the editable token value changes in the designer grid.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the token key edited by this row.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets or sets the editable token value shown in the designer grid.
    /// </summary>
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value)
                return;
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}

/// <summary>
/// Represents one read-only token definition row in the designer catalog.
/// </summary>
public sealed class TokenCatalogRow
{
    /// <summary>
    /// Creates a designer-friendly row from a core token definition.
    /// </summary>
    public TokenCatalogRow(ThemeTokenDefinition definition)
    {
        Group = definition.Group;
        Key = definition.Key;
        Type = definition.Type.ToString();
        DefaultValue = definition.DefaultValue;
        Required = definition.Required;
        Description = definition.Description;
    }

    /// <summary>
    /// Gets the token family shown in the catalog grid.
    /// </summary>
    public string Group { get; }

    /// <summary>
    /// Gets the token key shown in the catalog grid.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the token value type shown in the catalog grid.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the default value emitted for this token.
    /// </summary>
    public string DefaultValue { get; }

    /// <summary>
    /// Gets a value indicating whether the token is required for a complete theme.
    /// </summary>
    public bool Required { get; }

    /// <summary>
    /// Gets the token description shown in the catalog grid.
    /// </summary>
    public string Description { get; }
}
