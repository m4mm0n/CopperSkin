/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : samples\CopperSkin.SampleKitchenSink\App.xaml.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:39:53 +02:00
 *  Last Modified  : 2026-05-25 11:04:38 +02:00
 *  CRC32          : 996DDD55
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
// CRC32-BODY: 996DDD55
using System.Windows;
using System.IO;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using QuickLog;

namespace CopperSkin.SampleKitchenSink;

/// <summary>
/// Bootstraps a CopperSkin WPF application.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Configures logging, installs CopperSkin, and shows the main application window.
    /// </summary>
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

    /// <summary>
    /// Writes the shutdown log entry and flushes QuickLog before application exit.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        LogManager.Shutdown();
        base.OnExit(e);
    }
}
