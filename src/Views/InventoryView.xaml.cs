using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using SOCDOF.Data;

namespace SOCDOF.Views;

public partial class InventoryView : UserControl
{
    private readonly ObservableCollection<Product> _products = new();
    private readonly ICollectionView _productsView;

    public InventoryView()
    {
        InitializeComponent();
        _productsView = CollectionViewSource.GetDefaultView(_products);
        _productsView.Filter = FilterProduct;
        InventoryGrid.ItemsSource = _productsView;
        Loaded += InventoryView_OnLoaded;
    }

    private void InventoryView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= InventoryView_OnLoaded;
        LoadProducts();
    }

    private void LoadProducts()
    {
        try
        {
            using var database = AppDbContext.Create();
            var products = database.Products
                .AsNoTracking()
                .OrderBy(product => product.Name)
                .ToList();

            _products.Clear();
            foreach (var product in products)
            {
                _products.Add(product);
            }

            _productsView.Refresh();
            UpdateViewState();
        }
        catch (Exception exception)
        {
            ShowError("Die Lagerbestände konnten nicht geladen werden.", exception);
        }
    }

    private bool FilterProduct(object item)
    {
        if (item is not Product product)
        {
            return false;
        }

        var searchTerm = SearchBox?.Text.Trim();
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        return Contains(product.SKU, searchTerm)
            || Contains(product.Name, searchTerm)
            || Contains(product.Unit, searchTerm);
    }

    private static bool Contains(string? value, string searchTerm)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase);
    }

    private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _productsView?.Refresh();
        UpdateViewState();
    }

    private void StockInButton_OnClick(object sender, RoutedEventArgs e)
    {
        RecordStockMove(StockMoveType.In);
    }

    private void StockOutButton_OnClick(object sender, RoutedEventArgs e)
    {
        RecordStockMove(StockMoveType.Out);
    }

    private void RecordStockMove(StockMoveType type)
    {
        if (InventoryGrid.SelectedItem is not Product selectedProduct)
        {
            return;
        }

        var dialog = new StockMoveDialog(selectedProduct, type)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            using var database = AppDbContext.Create();
            using var transaction = database.Database.BeginTransaction();
            var product = database.Products.Single(existing => existing.Id == selectedProduct.Id);

            if (type == StockMoveType.Out && dialog.Quantity > product.StockQuantity)
            {
                transaction.Rollback();
                MessageBox.Show(
                    "Die Ausbuchungsmenge darf den aktuellen Bestand nicht überschreiten.",
                    AppConfig.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var quantityDelta = type == StockMoveType.In
                ? dialog.Quantity
                : -dialog.Quantity;
            product.StockQuantity += quantityDelta;
            database.StockMoves.Add(new StockMove
            {
                ProductId = product.Id,
                Quantity = dialog.Quantity,
                Type = type,
                Timestamp = DateTime.Now
            });
            database.SaveChanges();
            transaction.Commit();
            LoadProducts();
        }
        catch (InvalidOperationException)
        {
            LoadProducts();
            MessageBox.Show(
                "Das Produkt wurde zwischenzeitlich gelöscht.",
                AppConfig.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            ShowError("Die Lagerbewegung konnte nicht gespeichert werden.", exception);
        }
    }

    private void InventoryGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var hasSelection = InventoryGrid.SelectedItem is Product;
        StockInButton.IsEnabled = hasSelection;
        StockOutButton.IsEnabled = hasSelection;
    }

    private void UpdateViewState()
    {
        var hasProducts = _products.Count > 0;
        var hasVisibleProducts = _productsView.Cast<object>().Any();
        InventoryGrid.Visibility = hasProducts && hasVisibleProducts
            ? Visibility.Visible
            : Visibility.Collapsed;
        EmptyInventoryState.Visibility = hasProducts && hasVisibleProducts
            ? Visibility.Collapsed
            : Visibility.Visible;
        InventoryToolbar.Visibility = hasProducts ? Visibility.Visible : Visibility.Collapsed;
        StockInButton.IsEnabled = hasVisibleProducts && InventoryGrid.SelectedItem is Product;
        StockOutButton.IsEnabled = hasVisibleProducts && InventoryGrid.SelectedItem is Product;
        InventoryCountText.Text = hasProducts
            ? $"{_products.Count} Produkte im Lager"
            : "Noch keine Produkte";

        if (hasProducts && !hasVisibleProducts)
        {
            InventoryCountText.Text = "Keine Treffer für die aktuelle Suche";
        }
    }

    private static void ShowError(string message, Exception exception)
    {
        System.Diagnostics.Trace.TraceError("{0} {1}", message, exception);
        MessageBox.Show(
            message,
            AppConfig.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
