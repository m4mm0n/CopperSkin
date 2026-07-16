/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Wpf\CopperSkinThemeResources.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-07-05 00:00:00 +02:00
 *  Last Modified  : 2026-07-05 10:35:11 +02:00
 *  CRC32          : 4F3DE489
 *
 *  Description    :
 *                   Design-time friendly CopperSkin resource dictionary for WPF applications.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: 4F3DE489

using System.Windows;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf;

/// <summary>
/// Resource dictionary that loads CopperSkin styles and a resolved theme from XAML, including in the WPF designer.
/// </summary>
public sealed class CopperSkinThemeResources : ResourceDictionary
{

    /// <summary>
    /// Creates a design-time friendly CopperSkin dictionary using the built-in theme catalog.
    /// </summary>
    public CopperSkinThemeResources()
    {
        Refresh();
    }

    /// <summary>
    /// Gets or sets whether legacy amChipper token aliases are emitted for compatibility.
    /// </summary>
    public bool IncludeLegacyAliases
    {
        get => _includeLegacyAliases;
        set
        {
            _includeLegacyAliases = value;
            Refresh();
        }
    }

    /// <summary>
    /// Gets or sets whether bundled standard-control styles are merged for designer and runtime use.
    /// </summary>
    public bool MergeDefaultControlStyles
    {
        get => _mergeDefaultControlStyles;
        set
        {
            _mergeDefaultControlStyles = value;
            Refresh();
        }
    }

    private void Refresh()
    {
        Clear();
        MergedDictionaries.Clear();

        var options = new CopperSkinThemeOptions
        {
            ApplyNativeWindowChrome = false,
            IncludeLegacyAliases = IncludeLegacyAliases,
            MergeDefaultControlStyles = MergeDefaultControlStyles
        };

        if (MergeDefaultControlStyles)
            CopperSkinResourceEmitter.MergeDefaultDictionaries(this);

        var pack = ThemePack ?? BuiltInThemeCatalog.Create();
        var theme = _resolver.Resolve(pack, Theme);
        CopperSkinResourceEmitter.Apply(this, theme, options);
    }

    /// <summary>
    /// Gets or sets the theme name or id loaded into this dictionary.
    /// </summary>
    public string Theme
    {
        get => _theme;
        set
        {
            _theme = string.IsNullOrWhiteSpace(value) ? "FL Grape" : value;
            Refresh();
        }
    }

    /// <summary>
    /// Gets or sets the theme pack resolved by this resource dictionary. Built-in themes are used when unset.
    /// </summary>
    public ThemePack? ThemePack
    {
        get => _themePack;
        set
        {
            _themePack = value;
            Refresh();
        }
    }
    private bool _includeLegacyAliases = true;
    private bool _mergeDefaultControlStyles = true;
    private readonly ThemeResolver _resolver = new();
    private string _theme = "FL Grape";
    private ThemePack? _themePack;
}
