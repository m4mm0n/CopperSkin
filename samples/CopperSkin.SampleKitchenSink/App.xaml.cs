/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : samples\CopperSkin.SampleKitchenSink\App.xaml.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-05-25 09:39:53 +02:00
 *  Last Modified  : 2026-06-04 07:03:54 +02:00
 *  CRC32          : 32687F6B
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
// CRC32-BODY: 32687F6B
using System.Windows;
using System.Windows.Threading;
using System.IO;
using CopperSkin.Core.Theming;
using CopperSkin.Wpf;
using CopperSkin.Wpf.Controls;
using QuickLog;
using QuickLog.Exceptions;

namespace CopperSkin.SampleKitchenSink;

/// <summary>
/// Bootstraps a CopperSkin WPF application.
/// </summary>
public partial class App : Application
{
    private CopperSkinExceptionPopup? _exceptionPopup;

    /// <summary>
    /// Configures logging, installs CopperSkin, and shows the main application window.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CopperSkin", "SampleLogs");
        LogManager.ConfigureDefault(LoggerOptions.ForEngine(logDir).WithMinimumLevel(LogType.Info));
        LogManager.GetDefaultLogger().Log(LogType.Info, "CopperSkin sample starting");
        CopperSkinThemeManager.Install(this, BuiltInThemeCatalog.Create());
        _exceptionPopup = new CopperSkinExceptionPopup(() => MainWindow);
        LogManager.AttachExceptionHooks(new ExceptionHookOptions
        {
            CustomPopup = _exceptionPopup,
            MarkTaskExceptionsObserved = true,
            PopupTitle = "CopperSkin Sample Error",
            ShowStackTraceInPopup = false
        });
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        var window = new MainWindow();
        MainWindow = window;
        window.Show();
    }

    /// <summary>
    /// Writes the shutdown log entry and flushes QuickLog before application exit.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        DispatcherUnhandledException -= OnDispatcherUnhandledException;
        LogManager.GetDefaultLogger()?.Log(LogType.Info, "CopperSkin sample shutting down");
        LogManager.DetachExceptionHooks();
        LogManager.Shutdown();
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogManager.GetDefaultLogger()?.Log(LogType.Crit, "Unhandled WPF dispatcher exception", e.Exception);
        _exceptionPopup?.Show("CopperSkin Sample Error", FormatExceptionMessage(e.Exception), e.Exception, ExceptionSource.AppDomain);
        e.Handled = true;
    }

    private static string FormatExceptionMessage(Exception exception)
        => $"The exception was captured and logged with QuickLog.{Environment.NewLine}{Environment.NewLine}Type: {exception.GetType().FullName}{Environment.NewLine}Message: {exception.Message}";

    private sealed class CopperSkinExceptionPopup : IExceptionPopup
    {
        private readonly Func<Window?> _ownerProvider;

        public CopperSkinExceptionPopup(Func<Window?> ownerProvider)
            => _ownerProvider = ownerProvider;

        public void Show(string title, string message, Exception exception, ExceptionSource source)
        {
            var dispatcher = Current?.Dispatcher;
            if (dispatcher is not null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => ShowDialog(title, message, source));
                return;
            }

            ShowDialog(title, message, source);
        }

        private void ShowDialog(string title, string message, ExceptionSource source)
        {
            new CopperTaskDialog
            {
                Title = title,
                Heading = source == ExceptionSource.UnobservedTask
                    ? "An unobserved background task failed"
                    : "An unexpected error occurred",
                Text = message,
                PrimaryButtonText = "Close",
                Icon = source == ExceptionSource.UnobservedTask ? CopperTaskDialogIcon.Warning : CopperTaskDialogIcon.Error
            }.Show(_ownerProvider());
        }
    }
}
