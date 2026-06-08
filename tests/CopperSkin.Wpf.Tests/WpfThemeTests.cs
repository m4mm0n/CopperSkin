/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Wpf.Tests\WpfThemeTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : 7533BD52
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
// CRC32-BODY: 7533BD52
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Runtime.ExceptionServices;
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
    private static readonly object WpfLock = new();

    /// <summary>
    /// Verifies the Resource Emitter Creates Brushes And Gradients behavior.
    /// </summary>
    [WpfFact]
    public void ResourceEmitterCreatesBrushesAndGradients()
    {
        var dictionary = new ResourceDictionary();
        var theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "FL Grape");

        CopperSkinResourceEmitter.Apply(dictionary, theme);

        Assert.IsType<SolidColorBrush>(dictionary["BgDeep"]);
        Assert.IsType<LinearGradientBrush>(dictionary["CopperSkin.ChromeBrush"]);
        Assert.IsType<System.Windows.Media.Effects.DropShadowEffect>(dictionary["CopperSkin.SoftGlow"]);
        Assert.IsType<Thickness>(dictionary["CopperSkin.Spacing.Md"]);
        Assert.IsType<CornerRadius>(dictionary["CopperSkin.Radius.Md"]);
        Assert.IsType<Duration>(dictionary["CopperSkin.Motion.Transition"]);
    }

    /// <summary>
    /// Verifies the Drawing Snapshot Freezes Brushes behavior.
    /// </summary>
    [WpfFact]
    public void DrawingSnapshotFreezesBrushes()
    {
        var theme = new ThemeResolver().Resolve(BuiltInThemeCatalog.Create(), "Terminal Green");

        var snapshot = DrawingThemeSnapshot.FromTheme(theme);

        Assert.True(snapshot.Surface.IsFrozen);
        Assert.True(snapshot.GridLinePen.IsFrozen);
    }

    /// <summary>
    /// Verifies scoped themes can be removed without leaving stale subtree resources behind.
    /// </summary>
    [WpfFact]
    public void ThemeScopeRestoresResourcesWhenUnset()
    {
        RunOnSta(() =>
        {
            var application = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            var manager = CopperSkinThemeManager.Install(application, BuiltInThemeCatalog.Create());
            var element = new FrameworkElement();
            element.Resources["local.resource"] = "local";
            element.Resources["color.accent.primary"] = "local-accent";

            CopperSkinThemeScope.SetTheme(element, "Copper Desk");
            Assert.IsType<SolidColorBrush>(element.Resources["color.accent.primary"]);

            CopperSkinThemeScope.SetTheme(element, string.Empty);

            Assert.Equal("local", element.Resources["local.resource"]);
            Assert.Equal("local-accent", element.Resources["color.accent.primary"]);
            var merged = new ResourceDictionary { ["merged.resource"] = "merged" };
            element.Resources.MergedDictionaries.Add(merged);

            using (manager.BeginThemeScope(element, "Terminal Green"))
            {
                Assert.NotEmpty(element.Resources.Keys);
            }

            Assert.Single(element.Resources.MergedDictionaries);
            Assert.Same(merged, element.Resources.MergedDictionaries[0]);
        });
    }

    private static void RunOnSta(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                lock (WpfLock)
                    action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                Application.Current?.Shutdown();
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (exception is not null)
            ExceptionDispatchInfo.Capture(exception).Throw();
    }
}

/// <summary>
/// Marks WPF-focused facts that may later need STA-specific behavior.
/// </summary>
public sealed class WpfFactAttribute : FactAttribute
{
}
