using System.Windows;
using System.IO;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using QuickLog;

namespace CopperSkin.SampleKitchenSink;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CopperSkin", "SampleLogs");
        LogManager.ConfigureDefault(LoggerOptions.ForEngine(logDir).WithMinimumLevel(LogType.Info));
        LogManager.GetDefaultLogger().Log(LogType.Info, "CopperSkin sample starting");
        CopperSkinThemeManager.Install(this, BuiltInThemeCatalog.Create());
        var window = new MainWindow();
        MainWindow = window;
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        LogManager.Shutdown();
        base.OnExit(e);
    }
}
