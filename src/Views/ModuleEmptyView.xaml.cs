using System.Windows;
using System.Windows.Controls;

namespace SOCDOF.Views;

public partial class ModuleEmptyView : UserControl
{
    public ModuleEmptyView(string title, string description, string createLabel, string glyph)
    {
        InitializeComponent();
        EmptyTitle.Text = title;
        EmptyDescription.Text = description;
        CreateButton.Content = createLabel;
        ModuleGlyph.Text = glyph;
    }

    private void CreateButton_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Dieser Bereich ist für die nächste Ausbaustufe vorbereitet.",
            AppConfig.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
