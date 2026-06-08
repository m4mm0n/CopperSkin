/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinApp.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 3AE81D7B
 *
 *  Description    :
 *                   Fluent one-line CopperSkin WPF installation helpers.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: 3AE81D7B
using System.Windows;
using CopperSkin.Core.Theming;

namespace CopperSkin.Wpf;

/// <summary>
/// Entry point for concise CopperSkin WPF installation.
/// </summary>
public static class CopperSkinApp
{
    /// <summary>
    /// Starts a fluent CopperSkin installation for a WPF application.
    /// </summary>
    public static CopperSkinAppBuilder Use(Application application) => new(application);
}

/// <summary>
/// Fluent builder that installs CopperSkin with a theme pack and options.
/// </summary>
public sealed class CopperSkinAppBuilder
{
    private readonly Application _application;
    private ThemePack? _pack;
    private readonly CopperSkinThemeOptions _options = new();

    internal CopperSkinAppBuilder(Application application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
    }

    /// <summary>
    /// Selects the theme pack used by the runtime.
    /// </summary>
    public CopperSkinAppBuilder Pack(ThemePack pack)
    {
        _pack = pack;
        return this;
    }

    /// <summary>
    /// Selects the initial theme name or id.
    /// </summary>
    public CopperSkinAppBuilder Theme(string themeNameOrId)
    {
        _options.DefaultThemeName = themeNameOrId;
        return this;
    }

    /// <summary>
    /// Selects the preferred Windows 11 backdrop.
    /// </summary>
    public CopperSkinAppBuilder Backdrop(CopperSkinBackdropKind backdropKind)
    {
        _options.BackdropKind = backdropKind;
        return this;
    }

    /// <summary>
    /// Allows callers to tune installation options before install.
    /// </summary>
    public CopperSkinAppBuilder Configure(Action<CopperSkinThemeOptions> configure)
    {
        configure(_options);
        return this;
    }

    /// <summary>
    /// Installs CopperSkin and applies the selected initial theme.
    /// </summary>
    public CopperSkinThemeManager Install() => CopperSkinThemeManager.Install(_application, _pack, _options);
}
