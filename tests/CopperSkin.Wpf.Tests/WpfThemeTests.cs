/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Wpf.Tests\WpfThemeTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-05-25 11:25:22 +02:00
 *  CRC32          : 1A1A2BE0
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
// CRC32-BODY: 1A1A2BE0
using System.Windows;
using System.Windows.Media;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Drawing;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf.Tests;

/// <summary>
/// Verifies WPF resource emission and drawing-theme snapshot behavior.
/// </summary>
public sealed class WpfThemeTests
{
    /// <summary>
    /// Verifies the Resource Emitter Creates Brushes And Gradients behavior.
    /// </summary>
    [WpfFact]
    public void ResourceEmitterCreatesBrushesAndGradients()
    {
        var dictionary = new ResourceDictionary();
        ResolvedTheme theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "FL Grape");

        CopperSkinResourceEmitter.Apply(dictionary, theme);

        Assert.IsType<SolidColorBrush>(dictionary["BgDeep"]);
        Assert.IsType<LinearGradientBrush>(dictionary["CopperSkin.ChromeBrush"]);
        Assert.IsType<System.Windows.Media.Effects.DropShadowEffect>(dictionary["CopperSkin.SoftGlow"]);
    }

    /// <summary>
    /// Verifies the Drawing Snapshot Freezes Brushes behavior.
    /// </summary>
    [WpfFact]
    public void DrawingSnapshotFreezesBrushes()
    {
        ResolvedTheme theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "Terminal Green");

        DrawingThemeSnapshot snapshot = DrawingThemeSnapshot.FromTheme(theme);

        Assert.True(snapshot.Surface.IsFrozen);
        Assert.True(snapshot.GridLinePen.IsFrozen);
    }
}

/// <summary>
/// Marks WPF-focused facts that may later need STA-specific behavior.
/// </summary>
public sealed class WpfFactAttribute : FactAttribute
{
}
