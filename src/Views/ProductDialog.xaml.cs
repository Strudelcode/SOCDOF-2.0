using System.Globalization;
using System.Windows;
using SOCDOF.Data;

namespace SOCDOF.Views;

public partial class ProductDialog : Window
{
    private readonly int _productId;
    private readonly decimal _stockQuantity;

    public ProductDialog(Product? product = null)
    {
        InitializeComponent();
        Title = AppConfig.AppName;

        if (product is null)
        {
            StockBox.Text = 0m.ToString("N2", CultureInfo.CurrentCulture);
            return;
        }

        _productId = product.Id;
        _stockQuantity = product.StockQuantity;
        DialogTitle.Text = "Produkt bearbeiten";
        SaveButton.Content = "Änderungen speichern";
        SkuBox.Text = product.SKU;
        NameBox.Text = product.Name;
        PriceBox.Text = product.Price.ToString("N2", CultureInfo.CurrentCulture);
        UnitBox.Text = product.Unit;
        StockBox.Text = product.StockQuantity.ToString("N2", CultureInfo.CurrentCulture);
    }

    public Product ResultProduct { get; private set; } = null!;

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var sku = SkuBox.Text.Trim();
        var name = NameBox.Text.Trim();
        var unit = UnitBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(sku)
            || string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(unit))
        {
            ShowValidationError("SKU, Name und Einheit sind Pflichtfelder.");
            return;
        }

        if (!decimal.TryParse(PriceBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var price)
            || price < 0)
        {
            ShowValidationError("Bitte gib einen gültigen, nicht negativen Preis ein.");
            PriceBox.Focus();
            return;
        }

        ResultProduct = new Product
        {
            Id = _productId,
            SKU = sku,
            Name = name,
            Price = price,
            StockQuantity = _stockQuantity,
            Unit = unit
        };

        DialogResult = true;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void ShowValidationError(string message)
    {
        MessageBox.Show(
            message,
            AppConfig.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
