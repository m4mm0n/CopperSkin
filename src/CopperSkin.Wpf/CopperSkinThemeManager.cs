/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinThemeManager.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:34:20 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : C11CCC8F
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
// CRC32-BODY: C11CCC8F
using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Chrome;
using CopperSkin.Wpf.Drawing;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf;

/// <summary>
/// Installs CopperSkin resources, switches themes at runtime, and coordinates themed windows and drawing surfaces.
/// </summary>
public sealed class CopperSkinThemeManager
{
    private readonly Application _application;
    private readonly ThemePack _pack;
    /// <summary>
    /// Resolves themes from the active pack into runtime-ready token sets.
    /// </summary>
    private readonly ThemeResolver _resolver = new();
    private readonly CopperSkinThemeOptions _options;
    /// <summary>
    /// Applies CopperSkin colors to supported native DWM window chrome.
    /// </summary>
    private readonly WpfWindowChromeService _chromeService = new();

    /// <summary>
    /// Creates a manager bound to one WPF application and merges the default style dictionaries when requested.
    /// </summary>
    private CopperSkinThemeManager(Application application, ThemePack pack, CopperSkinThemeOptions? options)
    {
        _application = application;
        _pack = pack;
        _options = options ?? new CopperSkinThemeOptions();
        if (_options.MergeDefaultControlStyles)
            CopperSkinResourceEmitter.MergeDefaultDictionaries(_application.Resources);
    }

    /// <summary>
    /// Gets the application-wide CopperSkin theme manager currently installed.
    /// </summary>
    public static CopperSkinThemeManager? Current { get; private set; }

    /// <summary>
    /// Gets the theme pack owned by this manager.
    /// </summary>
    public ThemePack ThemePack => _pack;

    /// <summary>
    /// Gets the resolved theme currently applied to application resources.
    /// </summary>
    public ResolvedTheme? ActiveTheme { get; private set; }

    /// <summary>
    /// Raised after application resources, drawing surfaces, and attached windows receive a new theme.
    /// </summary>
    public event EventHandler<CopperSkinThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// Installs CopperSkin into a WPF application and applies the initial built-in theme.
    /// </summary>
    public static CopperSkinThemeManager Install(Application application, ThemePack? pack = null, CopperSkinThemeOptions? options = null)
    {
        if (application is null)
            throw new ArgumentNullException(nameof(application));

        Current = new CopperSkinThemeManager(application, pack ?? BuiltInThemeCatalog.Create(), options);
        Current.Apply("FL Grape");
        return Current;
    }

    /// <summary>
    /// Gets the display names of every theme available in the active theme pack.
    /// </summary>
    public IReadOnlyList<string> ThemeNames => _pack.Themes.Select(static t => t.Name).ToArray();

    /// <summary>
    /// Applies the requested CopperSkin theme, resource set, chrome color, or drawing snapshot.
    /// </summary>
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

    /// <summary>
    /// Applies a temporary theme to an element resource scope and restores previous resources on disposal.
    /// </summary>
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

    /// <summary>
    /// Attaches native chrome refresh hooks to a window.
    /// </summary>
    public void AttachWindow(Window window)
    {
        if (window is null)
            throw new ArgumentNullException(nameof(window));

        window.SourceInitialized += (_, _) => ApplyWindowChrome(window);
        window.Activated += (_, _) => ApplyWindowChrome(window);
        if (ActiveTheme is not null)
            ApplyWindowChrome(window);
    }

    /// <summary>
    /// Pushes the active theme into native window chrome when that integration is enabled.
    /// </summary>
    private void ApplyWindowChrome(Window window)
    {
        if (_options.ApplyNativeWindowChrome && ActiveTheme is not null)
            _chromeService.Apply(window, ActiveTheme);
    }

    /// <summary>
    /// Restores an element's resource dictionary after a temporary live-preview theme scope.
    /// </summary>
    private sealed class PreviewScope : IDisposable
    {
        private readonly FrameworkElement _target;
        private readonly ResourceDictionary _previous;
        private bool _disposed;

        /// <summary>
        /// Captures the target element and its previous resources for later restoration.
        /// </summary>
        public PreviewScope(FrameworkElement target, ResourceDictionary previous)
        {
            _target = target;
            _previous = previous;
        }

        /// <summary>
        /// Releases the scope and restores or closes the owned WPF resource.
        /// </summary>
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
