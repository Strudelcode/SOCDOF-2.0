using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SOCDOF.Views;

namespace SOCDOF;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ImageSource? _applicationIconSource;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ImageSource? ApplicationIconSource
    {
        get => _applicationIconSource;
        private set
        {
            if (ReferenceEquals(_applicationIconSource, value))
            {
                return;
            }

            _applicationIconSource = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ApplicationIconSource)));
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        LoadOptionalBranding();
        Title = AppConfig.AppName;
        SidebarAppName.Text = AppConfig.AppName;
        PageAppName.Text = AppConfig.AppName;
        SidebarVersion.Text = AppConfig.Version;
        ModuleContent.Content = new DashboardView();
    }

    private void LoadOptionalBranding()
    {
        foreach (var assetName in new[] { "app.ico", "logo.png" })
        {
            try
            {
                var assetUri = new Uri($"pack://application:,,,/src/Assets/{assetName}", UriKind.Absolute);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = assetUri;
                image.EndInit();
                AppLogo.Source = image;
                ApplicationIconSource = image;
                return;
            }
            catch (IOException)
            {
                // Continue with the next optional branding asset.
            }
            catch (InvalidOperationException)
            {
                // Continue when WPF cannot resolve an optional resource URI.
            }
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
