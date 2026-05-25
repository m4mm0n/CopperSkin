using System.Windows;
using System.Windows.Media;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Drawing;
using CopperSkin.Wpf.Resources;

namespace CopperSkin.Wpf.Tests;

public sealed class WpfThemeTests
{
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

    [WpfFact]
    public void DrawingSnapshotFreezesBrushes()
    {
        ResolvedTheme theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "Terminal Green");

        DrawingThemeSnapshot snapshot = DrawingThemeSnapshot.FromTheme(theme);

        Assert.True(snapshot.Surface.IsFrozen);
        Assert.True(snapshot.GridLinePen.IsFrozen);
    }
}

public sealed class WpfFactAttribute : FactAttribute
{
}
