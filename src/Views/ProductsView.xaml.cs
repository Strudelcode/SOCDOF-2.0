using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using SOCDOF.Data;
using SOCDOF.Services;

namespace SOCDOF.Views;

public partial class ProductsView : UserControl
{
    private readonly ObservableCollection<Product> _products = new();
    private readonly ICollectionView _productsView;

    public ProductsView()
    {
        InitializeComponent();
        _productsView = CollectionViewSource.GetDefaultView(_products);
        _productsView.Filter = FilterProduct;
        ProductGrid.ItemsSource = _productsView;
        Loaded += ProductsView_OnLoaded;
    }

    private void ProductsView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ProductsView_OnLoaded;
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
            ShowError("Die Produkte konnten nicht geladen werden.", exception);
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

    private void NewProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ProductDialog
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
            database.Products.Add(dialog.ResultProduct);
            database.SaveChanges();
            LoadProducts();
        }
        catch (DbUpdateException exception)
        {
            ShowError("Das Produkt konnte nicht angelegt werden. SKU und Name müssen eindeutig gültig sein.", exception);
        }
        catch (Exception exception)
        {
            ShowError("Das Produkt konnte nicht angelegt werden.", exception);
        }
    }

    private void EditProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ProductGrid.SelectedItem is not Product selectedProduct)
        {
            return;
        }

        var dialog = new ProductDialog(selectedProduct)
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
            var product = database.Products.Single(existing => existing.Id == selectedProduct.Id);
            product.SKU = dialog.ResultProduct.SKU;
            product.Name = dialog.ResultProduct.Name;
            product.Price = dialog.ResultProduct.Price;
            product.Unit = dialog.ResultProduct.Unit;
            database.SaveChanges();
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
        catch (DbUpdateException exception)
        {
            ShowError("Das Produkt konnte nicht gespeichert werden. Die SKU muss eindeutig sein.", exception);
        }
        catch (Exception exception)
        {
            ShowError("Das Produkt konnte nicht gespeichert werden.", exception);
        }
    }

    private void AmazonCartButton_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedProducts = ProductGrid.SelectedItems
            .OfType<Product>()
            .ToList();

        if (selectedProducts.Count == 0)
        {
            return;
        }

        try
        {
            var cartLink = AmazonCartLinkService.CreateCartLink(selectedProducts);
            AmazonCartLinkService.OpenCartLink(cartLink);
        }
        catch (Exception exception)
        {
            ShowError("Der Amazon Cart-Link konnte nicht geöffnet werden.", exception);
        }
    }

    private void DeleteProductButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ProductGrid.SelectedItem is not Product selectedProduct)
        {
            return;
        }

        var confirmation = MessageBox.Show(
            $"Möchtest du das Produkt '{selectedProduct.Name}' wirklich löschen?",
            AppConfig.AppName,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            using var database = AppDbContext.Create();
            var product = database.Products.Find(selectedProduct.Id);
            if (product is null)
            {
                LoadProducts();
                return;
            }

            database.Products.Remove(product);
            database.SaveChanges();
            LoadProducts();
        }
        catch (DbUpdateException exception)
        {
            ShowError("Das Produkt kann nicht gelöscht werden, weil es noch in Verkäufen oder Lagerbewegungen verwendet wird.", exception);
        }
        catch (Exception exception)
        {
            ShowError("Das Produkt konnte nicht gelöscht werden.", exception);
        }
    }

    private void ProductGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var hasSelection = ProductGrid.SelectedItem is Product;
        EditProductButton.IsEnabled = ProductGrid.SelectedItems.Count == 1 && hasSelection;
        DeleteProductButton.IsEnabled = ProductGrid.SelectedItems.Count == 1 && hasSelection;
        AmazonCartButton.IsEnabled = ProductGrid.SelectedItems.Count > 0;
    }

    private void UpdateViewState()
    {
        var hasProducts = _products.Count > 0;
        var hasVisibleProducts = _productsView.Cast<object>().Any();
        ProductGrid.Visibility = hasProducts && hasVisibleProducts
            ? Visibility.Visible
            : Visibility.Collapsed;
        EmptyProductState.Visibility = hasProducts && hasVisibleProducts
            ? Visibility.Collapsed
            : Visibility.Visible;
        ProductToolbar.Visibility = hasProducts ? Visibility.Visible : Visibility.Collapsed;
        EditProductButton.IsEnabled = hasVisibleProducts && ProductGrid.SelectedItems.Count == 1 && ProductGrid.SelectedItem is Product;
        DeleteProductButton.IsEnabled = hasVisibleProducts && ProductGrid.SelectedItems.Count == 1 && ProductGrid.SelectedItem is Product;
        AmazonCartButton.IsEnabled = hasVisibleProducts && ProductGrid.SelectedItems.Count > 0;
        ProductCountText.Text = hasProducts
            ? $"{_products.Count} Produkte"
            : "Noch keine Einträge";

        if (hasProducts && !hasVisibleProducts)
        {
            ProductCountText.Text = "Keine Treffer für die aktuelle Suche";
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
