using System.Diagnostics;
using System.Windows;
using SOCDOF.Data;
using SOCDOF.Services;

namespace SOCDOF;

public partial class App : Application
{
    private LocalApiServer? _localApiServer;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
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
            Trace.TraceError("{0} failed during startup: {1}", AppConfig.AppName, exception);
            Shutdown(-1);
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
}
