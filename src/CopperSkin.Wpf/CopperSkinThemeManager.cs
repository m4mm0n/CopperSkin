using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Chrome;
using CopperSkin.Wpf.Drawing;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf;

public sealed class CopperSkinThemeManager
{
    private readonly Application _application;
    private readonly ThemePack _pack;
    private readonly ThemeResolver _resolver = new();
    private readonly CopperSkinThemeOptions _options;
    private readonly WpfWindowChromeService _chromeService = new();

    private CopperSkinThemeManager(Application application, ThemePack pack, CopperSkinThemeOptions? options)
    {
        _application = application;
        _pack = pack;
        _options = options ?? new CopperSkinThemeOptions();
        if (_options.MergeDefaultControlStyles)
            CopperSkinResourceEmitter.MergeDefaultDictionaries(_application.Resources);
    }

    public static CopperSkinThemeManager? Current { get; private set; }

    public ThemePack ThemePack => _pack;

    public ResolvedTheme? ActiveTheme { get; private set; }

    public event EventHandler<CopperSkinThemeChangedEventArgs>? ThemeChanged;

    public static CopperSkinThemeManager Install(Application application, ThemePack? pack = null, CopperSkinThemeOptions? options = null)
    {
        if (application is null)
            throw new ArgumentNullException(nameof(application));

        Current = new CopperSkinThemeManager(application, pack ?? BuiltInThemeCatalog.Create(), options);
        Current.Apply("FL Grape");
        return Current;
    }

    public IReadOnlyList<string> ThemeNames => _pack.Themes.Select(static t => t.Name).ToArray();

    public ResolvedTheme Apply(string idOrName)
    {
        ResolvedTheme theme = _resolver.Resolve(_pack, idOrName);
        CopperSkinResourceEmitter.Apply(_application.Resources, theme, _options);
        ActiveTheme = theme;
        DrawingThemeRegistry.Apply(DrawingThemeSnapshot.FromTheme(theme));
        foreach (Window window in _application.Windows)
            ApplyWindowChrome(window);
        ThemeChanged?.Invoke(this, new CopperSkinThemeChangedEventArgs(theme));
        return theme;
    }

    public IDisposable BeginPreviewScope(FrameworkElement target, string idOrName)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));

        var previous = new ResourceDictionary();
        foreach (object key in target.Resources.Keys)
            previous[key] = target.Resources[key];

        ResolvedTheme theme = _resolver.Resolve(_pack, idOrName);
        CopperSkinResourceEmitter.Apply(target.Resources, theme, _options);
        return new PreviewScope(target, previous);
    }

    public void AttachWindow(Window window)
    {
        if (window is null)
            throw new ArgumentNullException(nameof(window));

        window.SourceInitialized += (_, _) => ApplyWindowChrome(window);
        window.Activated += (_, _) => ApplyWindowChrome(window);
        if (ActiveTheme is not null)
            ApplyWindowChrome(window);
    }

    private void ApplyWindowChrome(Window window)
    {
        if (_options.ApplyNativeWindowChrome && ActiveTheme is not null)
            _chromeService.Apply(window, ActiveTheme);
    }

    private sealed class PreviewScope : IDisposable
    {
        private readonly FrameworkElement _target;
        private readonly ResourceDictionary _previous;
        private bool _disposed;

        public PreviewScope(FrameworkElement target, ResourceDictionary previous)
        {
            _target = target;
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _target.Resources.Clear();
            foreach (object key in _previous.Keys)
                _target.Resources[key] = _previous[key];
            _disposed = true;
        }
    }
}
