using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace SOCDOF.Services;

public static class OfflineExportService
{
    public static void OfferEmailDraftExport(Window owner, string emlContent, string defaultFileName)
    {
        var choice = MessageBox.Show(
            owner,
            "Soll der E-Mail-Entwurf in die Zwischenablage kopiert werden? Mit 'Nein' kannst du ihn als .eml-Datei speichern.",
            AppConfig.AppName,
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Question,
            MessageBoxResult.Yes);

        if (choice == MessageBoxResult.Yes)
        {
            Clipboard.SetText(emlContent);
            MessageBox.Show(owner, "Der E-Mail-Entwurf wurde in die Zwischenablage kopiert.", AppConfig.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (choice == MessageBoxResult.No)
        {
            SaveTextFile(owner, emlContent, defaultFileName, "E-Mail-Dateien (*.eml)|*.eml|Alle Dateien (*.*)|*.*");
        }
    }

    public static void SaveCalendarFile(Window owner, string icsContent, string defaultFileName)
    {
        SaveTextFile(owner, icsContent, defaultFileName, "Kalenderdateien (*.ics)|*.ics|Alle Dateien (*.*)|*.*");
    }

    private static void SaveTextFile(Window owner, string content, string defaultFileName, string filter)
    {
        var dialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = Path.GetExtension(defaultFileName),
            FileName = defaultFileName,
            Filter = filter,
            OverwritePrompt = true,
            Title = "SOCDOF-Export speichern"
        };

        if (dialog.ShowDialog(owner) != true)
        {
            return;
        }

        try
        {
            File.WriteAllText(dialog.FileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            MessageBox.Show(owner, $"Export gespeichert: {dialog.FileName}", AppConfig.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Trace.TraceError("SOCDOF export failed: {0}", exception);
            MessageBox.Show(owner, "Der Export konnte nicht gespeichert werden.", AppConfig.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
