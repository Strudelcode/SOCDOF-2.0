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
                var resource = Application.GetResourceStream(assetUri);
                if (resource is null)
                {
                    continue;
                }

                using (resource.Stream)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = resource.Stream;
                    image.EndInit();
                    image.Freeze();
                    AppLogo.Source = image;
                    ApplicationIconSource = image;
                    return;
                }
            }
            catch (FileNotFoundException)
            {
                // An optional asset may be absent from a deployment.
            }
            catch (DirectoryNotFoundException)
            {
                // An optional asset directory may be absent from a deployment.
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Could not load optional branding asset '{assetName}': {exception.Message}", exception);
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
