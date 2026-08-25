using System.Windows;
using SOCDOF.Views;

namespace SOCDOF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Title = AppConfig.AppName;
        SidebarAppName.Text = AppConfig.AppName;
        PageAppName.Text = AppConfig.AppName;
        SidebarVersion.Text = AppConfig.Version;
        ShowModule("Dashboard", "Hier entsteht deine zentrale Übersicht.", "Dashboard öffnen", "⌂");
    }

    private void DashboardButton_OnClick(object sender, RoutedEventArgs e)
    {
        ShowModule("Dashboard", "Hier entsteht deine zentrale Übersicht.", "Dashboard öffnen", "⌂");
    }

    private void PartnersButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Partner";
        ModuleContent.Content = new PartnersView();
    }

    private void ProductsButton_OnClick(object sender, RoutedEventArgs e)
    {
        ShowModule("Produkte", "Lege Produkte an, um Sortiment und Preise lokal zu verwalten.", "Produkt erstellen", "▦");
    }

    private void SalesButton_OnClick(object sender, RoutedEventArgs e)
    {
        ShowModule("Verkäufe", "Lege einen Verkauf an, um Aufträge und Positionen zu erfassen.", "Verkauf erstellen", "◫");
    }

    private void InventoryButton_OnClick(object sender, RoutedEventArgs e)
    {
        ShowModule("Lager", "Lege eine Lagerbewegung an, um Ein- und Ausgänge zu dokumentieren.", "Lagerbewegung erstellen", "⇄");
    }

    private void ShowModule(string title, string description, string createLabel, string glyph)
    {
        PageTitle.Text = title;
        ModuleContent.Content = new ModuleEmptyView(title, description, createLabel, glyph);
    }
}
