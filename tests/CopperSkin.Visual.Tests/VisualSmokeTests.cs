/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : tests\CopperSkin.Visual.Tests\VisualSmokeTests.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:40:59 +02:00
 *  Last Modified  : 2026-05-25 11:53:16 +02:00
 *  CRC32          : 5084BDF9
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
// CRC32-BODY: 5084BDF9
using System.Runtime.ExceptionServices;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Threading;
using System.IO;
using CopperSkin.Core.Theming;
using CopperSkin.SampleKitchenSink;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Controls;
using QuickLog;

namespace CopperSkin.Visual.Tests;

/// <summary>
/// Verifies that the sample and designer visual shells can be constructed without XAML runtime failures.
/// </summary>
public sealed class VisualSmokeTests
{
    private const int WmContextMenu = 0x007B;
    private const int WmNcRButtonDown = 0x00A4;
    private const int WmSysCommand = 0x0112;
    private const int ScMouseMenu = 0xF090;
    private const int HtCaption = 2;

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
            var dropDownToggle = Assert.IsType<ToggleButton>(themeCombo.Template.FindName("DropDownToggle", themeCombo));
            var comboPopup = Assert.IsType<Popup>(themeCombo.Template.FindName("PART_Popup", themeCombo));
            Assert.NotNull(BindingOperations.GetBindingExpression(dropDownToggle, ToggleButton.IsCheckedProperty));
            Assert.NotNull(BindingOperations.GetBindingExpression(comboPopup, Popup.IsOpenProperty));
            Assert.Equal(PlacementMode.Bottom, comboPopup.Placement);

            CopperSkinThemeManager.Current!.AttachWindow(sample.Window);
            var copperWindow = Assert.IsAssignableFrom<CopperWindow>(sample.Window);
            Assert.NotNull(copperWindow.ContextMenu);
            Assert.Contains(copperWindow.ContextMenu.Items.OfType<MenuItem>(), item => item.Header?.ToString()?.StartsWith("About ", StringComparison.Ordinal) == true);

            copperWindow.ShowWindowContextMenu(new Point(copperWindow.Left + 24, copperWindow.Top + 32));
            Assert.True(copperWindow.ContextMenu.IsOpen);
            copperWindow.ContextMenu.IsOpen = false;

            nint hwnd = new WindowInteropHelper(copperWindow).Handle;
            nint titleBarPoint = PackScreenPoint(new Point(copperWindow.Left + 64, copperWindow.Top + 12));
            AssertNativeWindowContextMenuIsIgnored(copperWindow, WmContextMenu, hwnd, PackScreenPoint(copperWindow.PointToScreen(new Point(220, 220))));
            AssertNativeWindowContextMenuIsReplaced(copperWindow, WmNcRButtonDown, HtCaption, titleBarPoint);
            AssertNativeWindowContextMenuIsReplaced(copperWindow, WmContextMenu, hwnd, titleBarPoint);
            AssertNativeWindowContextMenuIsReplaced(copperWindow, WmSysCommand, ScMouseMenu, nint.Zero);
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
    /// Verifies that a native window-menu message is consumed and replaced by the themed CopperSkin menu.
    /// </summary>
    private static void AssertNativeWindowContextMenuIsReplaced(CopperWindow window, int message, nint wParam, nint lParam)
    {
        bool handled = InvokeWindowMessageHook(window, message, wParam, lParam);

        Assert.True(handled);
        Assert.True(window.ContextMenu?.IsOpen == true);
        window.ContextMenu!.IsOpen = false;
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
    }

    /// <summary>
    /// Verifies that a client-area context-menu message is left alone for WPF control-level handling.
    /// </summary>
    private static void AssertNativeWindowContextMenuIsIgnored(CopperWindow window, int message, nint wParam, nint lParam)
    {
        bool handled = InvokeWindowMessageHook(window, message, wParam, lParam);

        Assert.False(handled);
        Assert.False(window.ContextMenu?.IsOpen == true);
    }

    /// <summary>
    /// Invokes CopperWindow's private message hook and returns whether the hook consumed the message.
    /// </summary>
    private static bool InvokeWindowMessageHook(CopperWindow window, int message, nint wParam, nint lParam)
    {
        MethodInfo hook = typeof(CopperWindow).GetMethod("WindowMessageHook", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("CopperWindow message hook was not found.");
        object?[] args = [new WindowInteropHelper(window).Handle, message, wParam, lParam, false];

        hook.Invoke(window, args);

        return (bool)args[4]!;
    }

    /// <summary>
    /// Packs a screen coordinate into the LPARAM format used by non-client mouse messages.
    /// </summary>
    private static nint PackScreenPoint(Point point)
    {
        int x = (int)Math.Round(point.X);
        int y = (int)Math.Round(point.Y);
        return unchecked((nint)((ushort)x | ((ushort)y << 16)));
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
