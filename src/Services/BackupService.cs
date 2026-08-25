using System.Diagnostics;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace SOCDOF.Services;

public sealed class BackupService
{
    private const int MaximumBackupCount = 10;

    public string CreateStartupBackup()
    {
        AppConfig.EnsureDirectories();

        var backupPath = Path.Combine(
            AppConfig.BackupsDirectory,
            $"{AppConfig.StorageDirectoryName}_backup_{DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}.db");

        try
        {
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }

            using var source = new SqliteConnection(CreateConnectionString(AppConfig.DatabasePath));
            using var destination = new SqliteConnection(CreateConnectionString(backupPath));
            source.Open();
            destination.Open();
            source.BackupDatabase(destination);
        }
        catch (Exception exception)
        {
            Trace.TraceError("{0} database backup failed: {1}", AppConfig.AppName, exception);

            try
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }
            catch (Exception cleanupException)
            {
                Trace.TraceError("{0} could not remove an incomplete backup: {1}", AppConfig.AppName, cleanupException);
            }

            return string.Empty;
        }

        RemoveOlderBackups();
        return backupPath;
    }

    private static string CreateConnectionString(string databasePath)
    {
        return new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    private static void RemoveOlderBackups()
    {
        try
        {
            var backups = Directory.EnumerateFiles(
                    AppConfig.BackupsDirectory,
                    $"{AppConfig.StorageDirectoryName}_backup_*.db")
                .OrderByDescending(path => Path.GetFileName(path), StringComparer.Ordinal)
                .ToList();

            foreach (var oldBackup in backups.Skip(MaximumBackupCount))
            {
                File.Delete(oldBackup);
            }
        }
        catch (Exception exception)
        {
            Trace.TraceError("{0} backup retention cleanup failed: {1}", AppConfig.AppName, exception);
        }
    }
}
