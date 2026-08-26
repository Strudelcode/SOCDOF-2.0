using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using SOCDOF.Data;
using SOCDOF.Services;

namespace SOCDOF;

public partial class App : Application
{
    private LocalApiServer? _localApiServer;
    private int _fatalErrorShown;

    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);

            AppConfig.EnsureDirectories();

            using (var database = AppDbContext.Create())
            {
                database.Database.EnsureCreated();
                database.EnsureCurrentSchema();
                database.ConfigureWalMode();
            }

            new BackupService().CreateStartupBackup();

            if (AppConfig.LocalApiEnabled)
            {
                _localApiServer = new LocalApiServer();
                try
                {
                    _localApiServer.StartAsync().GetAwaiter().GetResult();
                }
                catch (Exception apiException)
                {
                    Trace.TraceError("{0} local API could not start: {1}", AppConfig.AppName, apiException);
                    _localApiServer.DisposeAsync().AsTask().GetAwaiter().GetResult();
                    _localApiServer = null;
                }
            }

            MainWindow = new MainWindow();
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            HandleFatalException("SOCDOF startup failed", exception, shutDown: true);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            _localApiServer?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        catch (Exception apiException)
        {
            Trace.TraceError("{0} local API could not stop cleanly: {1}", AppConfig.AppName, apiException);
        }
        finally
        {
            _localApiServer = null;
            base.OnExit(e);
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        HandleFatalException("Unhandled UI exception", e.Exception, shutDown: true);
        e.Handled = true;
    }

    private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception
            ?? new Exception(e.ExceptionObject?.ToString() ?? "Unknown unhandled exception.");

        HandleFatalException("Unhandled application exception", exception, shutDown: false);
    }

    private void HandleFatalException(string context, Exception exception, bool shutDown)
    {
        var message = $"{context}: {exception.Message}{Environment.NewLine}{Environment.NewLine}Details:{Environment.NewLine}{exception}";
        Trace.TraceError("{0}", message);
        WriteErrorLog(message);

        if (Interlocked.Exchange(ref _fatalErrorShown, 1) == 0)
        {
            try
            {
                MessageBox.Show(message, AppConfig.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception messageBoxException)
            {
                Trace.TraceError("{0} error dialog could not be displayed: {1}", AppConfig.AppName, messageBoxException);
            }
        }

        if (shutDown)
        {
            Shutdown(-1);
        }
    }

    private static void WriteErrorLog(string message)
    {
        try
        {
            AppConfig.EnsureDirectories();
            File.AppendAllText(
                AppConfig.ErrorLogPath,
                $"[{DateTimeOffset.Now:O}] {message}{Environment.NewLine}{Environment.NewLine}");
        }
        catch (Exception logException)
        {
            Trace.TraceError("{0} error log could not be written: {1}", AppConfig.AppName, logException);
        }
    }
}
