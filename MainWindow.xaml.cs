using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using SOCDOF.Views;

namespace SOCDOF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadOptionalLogo();
        Title = AppConfig.AppName;
        SidebarAppName.Text = AppConfig.AppName;
        PageAppName.Text = AppConfig.AppName;
        SidebarVersion.Text = AppConfig.Version;
        ModuleContent.Content = new DashboardView();
    }

    private void LoadOptionalLogo()
    {
        try
        {
            var logoUri = new Uri("pack://application:,,,/src/Assets/logo.png", UriKind.Absolute);
            var logo = new BitmapImage(logoUri);
            AppLogo.Source = logo;
            Icon = logo;
        }
        catch (IOException)
        {
            // The logo is optional until the supplied asset is synchronized into the project.
        }
        catch (InvalidOperationException)
        {
            // WPF can reject a resource URI when the optional asset is not part of the build.
        }
    }

    private void DashboardButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Dashboard";
        ModuleContent.Content = new DashboardView();
    }

    private void PartnersButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Partner";
        ModuleContent.Content = new PartnersView();
    }

    private void ProductsButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Produkte";
        ModuleContent.Content = new ProductsView();
    }

    private void SalesButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Verkäufe";
        ModuleContent.Content = new SaleOrdersView();
    }

    private void InventoryButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Lager";
        ModuleContent.Content = new InventoryView();
    }

    private void ShowModule(string title, string description, string createLabel, string glyph)
    {
        PageTitle.Text = title;
        ModuleContent.Content = new ModuleEmptyView(title, description, createLabel, glyph);
    }
}
