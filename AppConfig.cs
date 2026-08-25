using System.IO;

namespace SOCDOF;

public static class AppConfig
{
    public static string AppName = "SOCDOF";
    public static string Version = "v1.7.0";
    public static bool LocalApiEnabled = true;
    public static string LocalApiUrl = "http://localhost:5050";

    public static string AppDataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        AppName);

    public static string BackupsDirectory { get; } = Path.Combine(
        AppDataDirectory,
        "backups");

    public static string DatabasePath { get; } = Path.Combine(
        AppDataDirectory,
        $"{AppName}.db");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(AppDataDirectory);
        Directory.CreateDirectory(BackupsDirectory);
    }
}
