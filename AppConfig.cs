using System.IO;

namespace SOCDOF;

public static class AppConfig
{
    public static string AppName = "SOCDOF 2.0";
    public static string Version = "v2.0.0";
    public static string StorageDirectoryName = "SOCDOF";
    public static bool LocalApiEnabled = true;
    public static string LocalApiUrl = "http://localhost:5050";

    public static string AppDataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        StorageDirectoryName);

    public static string BackupsDirectory { get; } = Path.Combine(
        AppDataDirectory,
        "backups");

    public static string DatabasePath { get; } = Path.Combine(
        AppDataDirectory,
        $"{StorageDirectoryName}.db");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(AppDataDirectory);
        Directory.CreateDirectory(BackupsDirectory);
    }
}
