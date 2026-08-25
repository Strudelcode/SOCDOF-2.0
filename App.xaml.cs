using System.Diagnostics;
using System.Windows;
using SOCDOF.Data;
using SOCDOF.Services;

namespace SOCDOF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            AppConfig.EnsureDirectories();

            using (var database = AppDbContext.Create())
            {
                database.Database.EnsureCreated();
                database.ConfigureWalMode();
            }

            new BackupService().CreateStartupBackup();

            MainWindow = new MainWindow();
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            Trace.TraceError("SOCDOF failed during startup: {0}", exception);
            Shutdown(-1);
        }
    }
}
