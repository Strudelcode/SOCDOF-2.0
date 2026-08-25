using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SOCDOF.Data;

namespace SOCDOF.Views;

public sealed class SaleOrderPartnerOption
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class SaleOrderProductOption
{
    public int Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

public sealed class SaleOrderLineInput : INotifyPropertyChanged
{
    private int _productId;
    private decimal _quantity = 1m;
    private decimal _unitPrice;

    public int ProductId
    {
        get => _productId;
        set
        {
            if (_productId == value)
            {
                return;
            }

            _productId = value;
            OnPropertyChanged();
        }
    }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value)
            {
                return;
            }

            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (_unitPrice == value)
            {
                return;
            }

            _unitPrice = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LineTotal));
        }
    }

    public string ProductDisplay { get; set; } = "Produkt auswählen";
    public decimal LineTotal => Quantity * UnitPrice;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class SaleOrderDialog : Window
{
    private readonly ObservableCollection<SaleOrderLineInput> _lines = new();

    public SaleOrderDialog()
    {
        InitializeComponent();
        Title = AppConfig.AppName;
        PartnerBox.ItemsSource = LoadPartners();
        DataContext = this;
        LinesGrid.ItemsSource = _lines;
        TotalText.Text = FormatAmount(0m);
        AddLineButton.IsEnabled = false;
    }

    public ObservableCollection<SaleOrderProductOption> Products { get; } = new();
    public int PartnerId { get; private set; }
    public IReadOnlyList<SaleOrderLineInput> Lines => _lines;
    public DateTime? DeliveryDate => DeliveryDatePicker.SelectedDate;
    public decimal TotalAmount => _lines.Sum(line => line.LineTotal);

    private static List<SaleOrderPartnerOption> LoadPartners()
    {
        using var database = AppDbContext.Create();
        return database.Partners
            .OrderBy(partner => partner.Name)
            .Select(partner => new SaleOrderPartnerOption
            {
                Id = partner.Id,
                Name = partner.Name
            })
            .ToList();
    }

    private void PartnerBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PartnerBox.SelectedItem is not SaleOrderPartnerOption partner)
        {
            PartnerId = 0;
            Products.Clear();
            _lines.Clear();
            AddLineButton.IsEnabled = false;
            UpdateTotal();
            return;
        }

        PartnerId = partner.Id;
        LoadProducts();
        AddLineButton.IsEnabled = Products.Count > 0;
        if (_lines.Count == 0 && Products.Count > 0)
        {
            AddLine();
        }
    }

    private void LoadProducts()
    {
        using var database = AppDbContext.Create();
        var products = database.Products
            .OrderBy(product => product.Name)
            .Select(product => new SaleOrderProductOption
            {
                Id = product.Id,
                DisplayName = $"{product.SKU} · {product.Name}",
                Price = product.Price
            })
            .ToList();

        Products.Clear();
        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    private void AddLineButton_OnClick(object sender, RoutedEventArgs e)
    {
        AddLine();
    }

    private void AddLine()
    {
        if (Products.Count == 0)
        {
            return;
        }

        var product = Products[0];
        var line = new SaleOrderLineInput
        {
            ProductId = product.Id,
            ProductDisplay = product.DisplayName,
            UnitPrice = product.Price
        };
        line.PropertyChanged += Line_OnPropertyChanged;
        _lines.Add(line);
        UpdateTotal();
    }

    private void RemoveLineButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not SaleOrderLineInput line)
        {
            return;
        }

        line.PropertyChanged -= Line_OnPropertyChanged;
        _lines.Remove(line);
        UpdateTotal();
    }

    private void ProductBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.DataContext is not SaleOrderLineInput line)
        {
            return;
        }

        if (comboBox.SelectedItem is not SaleOrderProductOption product)
        {
            return;
        }

        line.ProductId = product.Id;
        line.ProductDisplay = product.DisplayName;
        line.UnitPrice = product.Price;
        LinesGrid.Items.Refresh();
        UpdateTotal();
    }

    private void Line_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SaleOrderLineInput.Quantity)
            or nameof(SaleOrderLineInput.UnitPrice)
            or nameof(SaleOrderLineInput.LineTotal))
        {
            UpdateTotal();
        }
    }

    private void UpdateTotal()
    {
        TotalText.Text = FormatAmount(TotalAmount);
    }

    private void CompleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        ValidationText.Text = string.Empty;

        if (PartnerId == 0)
        {
            ValidationText.Text = "Bitte wähle einen Partner aus.";
            return;
        }

        if (_lines.Count == 0)
        {
            ValidationText.Text = "Füge mindestens eine Produktposition hinzu.";
            return;
        }

        if (_lines.Any(line => line.ProductId == 0 || line.Quantity <= 0 || line.UnitPrice < 0))
        {
            ValidationText.Text = "Jede Position benötigt ein Produkt, eine positive Menge und einen gültigen Einzelpreis.";
            return;
        }

        DialogResult = true;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static string FormatAmount(decimal amount)
    {
        return amount.ToString("N2", CultureInfo.CurrentCulture);
    }
}
