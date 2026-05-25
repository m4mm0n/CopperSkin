using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using System.IO;
using CopperSkin.Core.Theming;
using CopperSkin.SampleKitchenSink;
using CopperSkin.Wpf;
using QuickLog;

namespace CopperSkin.Visual.Tests;

public sealed class VisualSmokeTests
{
    [Fact]
    public void SampleRowsAreAvailable()
    {
        var row = new ControlRow("Window", "DWM aware");

        Assert.Equal("Window", row.Name);
        Assert.Equal("DWM aware", row.State);
    }

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
        });
    }

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

    private sealed class WindowScope : IDisposable
    {
        public WindowScope(Window window)
        {
            Window = window;
        }

        public Window Window { get; }

        public void Dispose() => Window.Close();
    }
}
