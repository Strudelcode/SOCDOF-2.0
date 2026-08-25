using System.Globalization;
using System.Windows;
using SOCDOF.Data;

namespace SOCDOF.Views;

public partial class StockMoveDialog : Window
{
    public StockMoveDialog(Product product, StockMoveType type)
    {
        InitializeComponent();
        Title = AppConfig.AppName;
        DialogTitle.Text = type == StockMoveType.In ? "Wareneingang buchen" : "Warenausgang buchen";
        ProductSummary.Text = $"{product.SKU} · {product.Name} · Aktueller Bestand: {product.StockQuantity.ToString("N2", CultureInfo.CurrentCulture)} {product.Unit}";
        QuantityBox.Focus();
    }

    public decimal Quantity { get; private set; }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(QuantityBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var quantity)
            || quantity <= 0)
        {
            MessageBox.Show(
                "Bitte gib eine positive Menge ein.",
                AppConfig.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            QuantityBox.Focus();
            return;
        }

        Quantity = quantity;
        DialogResult = true;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
