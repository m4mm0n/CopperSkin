/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Wpf.Tests\WpfThemeTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-07-05 10:39:50 +02:00
 *  CRC32          : 7C388A03
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
// CRC32-BODY: 7C388A03

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
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

    private static int CountDefaultStyleDictionaries(ResourceDictionary resources)
    {
        var count = resources.Contains("CopperSkin.Internal.DefaultControlStyles")
            || resources.Source?.OriginalString.Contains("/CopperSkin.Wpf;component/Themes/CopperSkin.Controls.xaml", StringComparison.OrdinalIgnoreCase) == true
            ? 1
            : 0;
        foreach (var dictionary in resources.MergedDictionaries)
            count += CountDefaultStyleDictionaries(dictionary);
        return count;
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
    /// Verifies runtime install reuses design-time dictionaries instead of merging duplicate bundled styles.
    /// </summary>
    [WpfFact]
    public void ResourceEmitterDoesNotDuplicateDesignerStyles()
    {
        RunOnSta(() =>
        {
            var resources = new ResourceDictionary();
            resources.MergedDictionaries.Add(new CopperSkinThemeResources { Theme = "FL Grape" });
            var before = CountDefaultStyleDictionaries(resources);

            CopperSkinResourceEmitter.MergeDefaultDictionaries(resources);

            Assert.Equal(before, CountDefaultStyleDictionaries(resources));
        });
    }

    /// <summary>
    /// Verifies horizontal scrollbars move the thumb in the same direction as the value.
    /// </summary>
    [WpfFact]
    public void HorizontalScrollBarTemplateUsesForwardDirection()
    {
        RunOnSta(() =>
        {
            var resources = new CopperSkinThemeResources { Theme = "Terminal Green" };
            var scrollBar = new ScrollBar
            {
                Orientation = Orientation.Horizontal,
                Style = (Style)resources[typeof(ScrollBar)]
            };

            Assert.True(scrollBar.ApplyTemplate());

            var track = (Track?)scrollBar.Template.FindName("PART_Track", scrollBar);
            Assert.NotNull(track);
            Assert.False(track!.IsDirectionReversed);
        });
    }

    /// <summary>
    /// Verifies the first v0.3 primitive and media control coverage wave is available in the public resource scope.
    /// </summary>
    [WpfFact]
    public void ThemeResourcesCoverV03PrimitiveAndMediaControls()
    {
        RunOnSta(() =>
        {
            var resources = new CopperSkinThemeResources { Theme = "Terminal Green" };

            Assert.IsType<Style>(resources[typeof(BulletDecorator)]);
            Assert.IsType<Style>(resources[typeof(Image)]);
            Assert.IsType<Style>(resources[typeof(InkPresenter)]);
            Assert.IsType<Style>(resources[typeof(MediaElement)]);
        });
    }

    /// <summary>
    /// Verifies the v0.3 interaction surfaces retain their standard WPF templates and keyboard state.
    /// </summary>
    [WpfFact]
    public void ThemeResourcesApplyToInteractionControlWave()
    {
        RunOnSta(() =>
        {
            var resources = new CopperSkinThemeResources { Theme = "Terminal Green" };

            var richTextBox = new RichTextBox { Style = (Style)resources[typeof(RichTextBox)], IsReadOnly = true };
            var datePicker = new DatePicker { Style = (Style)resources[typeof(DatePicker)], IsTodayHighlighted = true };
            var dataGrid = new DataGrid { Style = (Style)resources[typeof(DataGrid)], CanUserAddRows = false };
            var tabControl = new TabControl { Style = (Style)resources[typeof(TabControl)] };
            var toolbarOverflow = new ToolBarOverflowPanel { Style = (Style)resources[typeof(ToolBarOverflowPanel)] };
            var treeItem = new TreeViewItem { Style = (Style)resources[typeof(TreeViewItem)] };

            Assert.True(richTextBox.IsReadOnly);
            Assert.True(datePicker.IsTodayHighlighted);
            Assert.False(dataGrid.CanUserAddRows);
            Assert.True(richTextBox.ApplyTemplate());
            Assert.NotNull(datePicker.Style);
            Assert.NotNull(dataGrid.Style);
            Assert.NotNull(tabControl.Style);
            Assert.NotNull(toolbarOverflow.Style);
            Assert.NotNull(treeItem.FocusVisualStyle);
        });
    }

    /// <summary>
    /// Verifies the v0.3 partial-control wave exposes focus, validation, keyboard, and state contracts.
    /// </summary>
    [WpfFact]
    public void ThemeResourcesCompletePartialControlBehaviorContracts()
    {
        RunOnSta(() =>
        {
            var resources = new CopperSkinThemeResources { Theme = "Terminal Green" };

            Assert.IsType<Style>(resources[typeof(DataGrid)]);
            Assert.IsType<Style>(resources[typeof(RichTextBox)]);
            Assert.IsType<Style>(resources[typeof(DatePicker)]);
            Assert.IsType<Style>(resources[typeof(Calendar)]);
            Assert.IsType<Style>(resources[typeof(TabControl)]);
            Assert.IsType<Style>(resources[typeof(TabItem)]);
            Assert.IsType<Style>(resources[typeof(ToolBarOverflowPanel)]);
            Assert.IsType<ControlTemplate>(resources["CopperSkin.ValidationErrorTemplate"]);

            var dataGrid = (Style)resources[typeof(DataGrid)];
            var richTextBox = (Style)resources[typeof(RichTextBox)];
            var datePicker = (Style)resources[typeof(DatePicker)];
            var tabControl = (Style)resources[typeof(TabControl)];

            Assert.NotNull(FindSetter(dataGrid, Control.FocusVisualStyleProperty));
            Assert.NotNull(FindSetter(dataGrid, Validation.ErrorTemplateProperty));
            Assert.NotNull(FindSetter(dataGrid, KeyboardNavigation.TabNavigationProperty));
            Assert.NotNull(FindSetter(richTextBox, Control.FocusVisualStyleProperty));
            Assert.NotNull(FindSetter(richTextBox, Validation.ErrorTemplateProperty));
            Assert.NotNull(FindSetter(datePicker, Control.FocusVisualStyleProperty));
            Assert.NotNull(FindSetter(datePicker, Validation.ErrorTemplateProperty));
            Assert.NotNull(FindSetter(tabControl, KeyboardNavigation.TabNavigationProperty));
            Assert.NotNull(FindSetter(tabControl, KeyboardNavigation.ControlTabNavigationProperty));

            var readOnlyEditor = new RichTextBox { Style = richTextBox, IsReadOnly = true, FlowDirection = FlowDirection.RightToLeft };
            var rightToLeftGrid = new DataGrid { Style = dataGrid, IsReadOnly = true, FlowDirection = FlowDirection.RightToLeft };
            var disabledDatePicker = new DatePicker { Style = datePicker, IsEnabled = false, FlowDirection = FlowDirection.RightToLeft };

            Assert.True(readOnlyEditor.IsReadOnly);
            Assert.Equal(FlowDirection.RightToLeft, readOnlyEditor.FlowDirection);
            Assert.True(rightToLeftGrid.IsReadOnly);
            Assert.Equal(FlowDirection.RightToLeft, rightToLeftGrid.FlowDirection);
            Assert.False(disabledDatePicker.IsEnabled);
            Assert.Equal(FlowDirection.RightToLeft, disabledDatePicker.FlowDirection);
        });
    }

    /// <summary>Verifies themed resources change their resolved palette without replacing control behavior.</summary>
    [WpfFact]
    public void ThemeResourcesSwitchThemeForPartialControls()
    {
        RunOnSta(() =>
        {
            var resources = new CopperSkinThemeResources { Theme = "Terminal Green" };
            var initial = ((SolidColorBrush)resources["color.accent.primary"]).Color;
            var controlStyle = (Style)resources[typeof(DataGrid)];

            resources.Theme = "FL Grape";

            var switched = ((SolidColorBrush)resources["color.accent.primary"]).Color;
            Assert.NotEqual(initial, switched);
            Assert.NotSame(controlStyle, resources[typeof(DataGrid)]);
            Assert.NotNull(FindSetter((Style)resources[typeof(DataGrid)], Validation.ErrorTemplateProperty));
            Assert.NotNull(FindSetter((Style)resources[typeof(TabItem)], Control.FocusVisualStyleProperty));
        });
    }

    private static Setter? FindSetter(Style style, DependencyProperty property) =>
        style.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property == property);

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
                var application = Application.Current;
                if (application is not null && application.Dispatcher.CheckAccess())
                    application.Shutdown();
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (exception is not null)
            ExceptionDispatchInfo.Capture(exception).Throw();
    }

    /// <summary>
    /// Verifies XAML-loadable CopperSkin resources theme standard controls before app startup runs.
    /// </summary>
    [WpfFact]
    public void ThemeResourcesLoadForDesignerSurfaces()
    {
        RunOnSta(() =>
        {
            var resources = new CopperSkinThemeResources { Theme = "Terminal Green" };

            Assert.IsType<SolidColorBrush>(resources["color.accent.primary"]);
            Assert.True(CountDefaultStyleDictionaries(resources) > 0);
        });
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
    private static readonly object WpfLock = new();
}

/// <summary>
/// Marks WPF-focused facts that may later need STA-specific behavior.
/// </summary>
public sealed class WpfFactAttribute : FactAttribute
{
}
