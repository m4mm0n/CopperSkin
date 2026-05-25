using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Controls;
using Microsoft.Win32;
using QuickLog;

namespace CopperSkin.Designer;

public partial class MainWindow : CopperWindow, INotifyPropertyChanged
{
    private readonly ThemePack _runtimePack = BuiltInThemeCatalog.Create();
    private ThemeDefinition? _selectedTheme;
    private TokenRow? _selectedToken;

    public MainWindow()
    {
        InitializeComponent();
        Themes = new ObservableCollection<ThemeDefinition>(_runtimePack.Themes.Select(static t => t.Clone()));
        Diagnostics = new ObservableCollection<string>();
        Tokens = new ObservableCollection<TokenRow>();
        DataContext = this;
        ThemeList.SelectedIndex = 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<ThemeDefinition> Themes { get; }

    public ObservableCollection<TokenRow> Tokens { get; }

    public ObservableCollection<string> Diagnostics { get; }

    public TokenRow? SelectedToken
    {
        get => _selectedToken;
        set { _selectedToken = value; OnPropertyChanged(); }
    }

    public string StatusText { get; private set; } = "Ready";

    private void ThemeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        _selectedTheme = ThemeList.SelectedItem as ThemeDefinition;
        LoadSelectedTheme();
    }

    private void LoadSelectedTheme()
    {
        if (_selectedTheme is null)
            return;

        Tokens.Clear();
        foreach (var pair in _selectedTheme.Tokens.OrderBy(static t => t.Key, StringComparer.OrdinalIgnoreCase))
            Tokens.Add(new TokenRow(pair.Key, pair.Value));

        ApplyCurrentTheme();
    }

    private void ApplyToken_Click(object sender, RoutedEventArgs e) => ApplyTokenEdit();

    private void TokenValueBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ApplyTokenEdit();

    private void ApplyTokenEdit()
    {
        if (_selectedTheme is null || SelectedToken is null)
            return;

        _selectedTheme.Tokens[SelectedToken.Key] = SelectedToken.Value;
        ApplyCurrentTheme();
    }

    private void ApplyCurrentTheme()
    {
        if (_selectedTheme is null)
            return;

        var tempPack = new ThemePack { Id = "designer.preview", Name = "Designer Preview", Themes = [_selectedTheme.Clone()] };
        var diagnostics = new ThemeValidator().Validate(tempPack);
        Diagnostics.Clear();
        foreach (ThemeDiagnostic diagnostic in diagnostics)
            Diagnostics.Add(diagnostic.ToString());

        var resolved = new ThemeResolver().Resolve(tempPack, _selectedTheme.Id);
        CopperSkin.Wpf.Resources.CopperSkinResourceEmitter.Apply(Application.Current.Resources, resolved);
        CopperSkin.Wpf.Drawing.DrawingThemeRegistry.Apply(CopperSkin.Wpf.Drawing.DrawingThemeSnapshot.FromTheme(resolved));
        StatusText = $"Previewing {_selectedTheme.Name} - {diagnostics.Count} diagnostics";
        OnPropertyChanged(nameof(StatusText));
    }

    private void DuplicateTheme_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedTheme is null)
            return;

        ThemeDefinition copy = _selectedTheme.Clone();
        copy.Name = $"{copy.Name} Custom";
        copy.Id = ThemeNames.Slug(copy.Name);
        Themes.Add(copy);
        ThemeList.SelectedItem = copy;
        LogManager.GetDefaultLogger().Log(LogType.Info, $"Duplicated theme {copy.Name}");
    }

    private void ValidatePack_Click(object sender, RoutedEventArgs e)
    {
        var pack = new ThemePack { Id = "designer.current", Name = "Designer Current" };
        pack.Themes.AddRange(Themes.Select(static t => t.Clone()));
        Diagnostics.Clear();
        foreach (ThemeDiagnostic diagnostic in new ThemeValidator().Validate(pack))
            Diagnostics.Add(diagnostic.ToString());
        StatusText = $"Validated {pack.Themes.Count} themes";
        OnPropertyChanged(nameof(StatusText));
    }

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
    }

    private void TaskDialog_Click(object sender, RoutedEventArgs e)
    {
        new CopperTaskDialog
        {
            Title = "CopperSkin TaskDialog",
            Heading = "Live themed dialog",
            Text = "This is a CopperSkin-controlled TaskDialog-style surface using the active theme.",
            ShowSecondaryButton = true
        }.Show(this);
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public sealed class TokenRow : INotifyPropertyChanged
{
    private string _value;

    public TokenRow(string key, string value)
    {
        Key = key;
        _value = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Key { get; }

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
