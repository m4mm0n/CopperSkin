/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Visual.Tests\VisualSmokeTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-05-25 11:24:08 +02:00
 *  CRC32          : D03BEC61
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
// CRC32-BODY: D03BEC61
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using CopperSkin.Core.Theming;
using CopperSkin.SampleKitchenSink;
using CopperSkin.Wpf;
using QuickLog;

namespace CopperSkin.Visual.Tests;

/// <summary>
/// Verifies that the sample and designer visual shells can be constructed without XAML runtime failures.
/// </summary>
public sealed class VisualSmokeTests
{
    /// <summary>
    /// Verifies the Sample Rows Are Available behavior.
    /// </summary>
    [Fact]
    public void SampleRowsAreAvailable()
    {
        var row = new ControlRow("Window", "DWM aware");

        Assert.Equal("Window", row.Name);
        Assert.Equal("DWM aware", row.State);
    }

    /// <summary>
    /// Verifies the Designer And Sample Main Windows Load Xaml behavior.
    /// </summary>
    [Fact]
    public void DesignerAndSampleMainWindowsLoadXaml()
    {
        RunOnSta(() =>
        {
            string logDir = Path.Combine(Path.GetTempPath(), "copperskin-visual-test-" + Guid.NewGuid().ToString("N"));
            LogManager.ConfigureDefault(LoggerOptions.ForTool("copperskin-visual-test").WithJsonLog(Path.Combine(logDir, "visual.jsonl")));

            var app = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            CopperSkinThemeManager.Install(app, BuiltInThemeCatalog.Create());

            using var designer = new WindowScope(new CopperSkin.Designer.MainWindow());
            using var sample = new WindowScope(new CopperSkin.SampleKitchenSink.MainWindow());

            Assert.Equal("CopperSkin Designer", designer.Window.Title);
            Assert.Equal("CopperSkin Kitchen Sink", sample.Window.Title);

            sample.Window.Show();
            sample.Window.UpdateLayout();
            var themeCombo = Assert.IsType<ComboBox>(sample.Window.FindName("ThemeCombo"));
            themeCombo.ApplyTemplate();
            var toggle = Assert.IsType<ToggleButton>(themeCombo.Template.FindName("DropDownToggle", themeCombo));
            toggle.IsChecked = true;
            Assert.True(themeCombo.IsDropDownOpen);
            themeCombo.IsDropDownOpen = false;

            CopperSkinThemeManager.Current!.AttachWindow(sample.Window);
            Assert.NotNull(sample.Window.ContextMenu);
            Assert.Contains(sample.Window.ContextMenu.Items.OfType<MenuItem>(), item => item.Header?.ToString()?.StartsWith("About ", StringComparison.Ordinal) == true);
        });
    }

    /// <summary>
    /// Runs a visual smoke-test action on an STA thread suitable for WPF.
    /// </summary>
    private static void RunOnSta(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                LogManager.Shutdown();
                Application.Current?.Shutdown();
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception is not null)
            ExceptionDispatchInfo.Capture(exception).Throw();
    }

    /// <summary>
    /// Closes a WPF window at the end of a visual smoke-test scope.
    /// </summary>
    private sealed class WindowScope : IDisposable
    {
        /// <summary>
        /// Verifies the Window Scope behavior.
        /// </summary>
        public WindowScope(Window window)
        {
            Window = window;
        }

        /// <summary>
        /// Gets the window owned by the smoke-test scope.
        /// </summary>
        public Window Window { get; }

        /// <summary>
        /// Releases the scope and restores or closes the owned WPF resource.
        /// </summary>
        public void Dispose() => Window.Close();
    }
}
