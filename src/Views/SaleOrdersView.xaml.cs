using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using SOCDOF.Data;
using SOCDOF.Services;

namespace SOCDOF.Views;

public sealed class SaleOrderListItem
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public DateTime? DeliveryDate { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}

public partial class SaleOrdersView : UserControl
{
    private readonly ObservableCollection<SaleOrderListItem> _orders = new();
    private readonly ICollectionView _ordersView;
    private SaleOrder? _selectedOrder;

    public SaleOrdersView()
    {
        InitializeComponent();
        _ordersView = CollectionViewSource.GetDefaultView(_orders);
        _ordersView.Filter = FilterOrder;
        OrderGrid.ItemsSource = _ordersView;
        Loaded += SaleOrdersView_OnLoaded;
    }

    private void SaleOrdersView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SaleOrdersView_OnLoaded;
        LoadOrders();
    }

    private void LoadOrders()
    {
        try
        {
            using var database = AppDbContext.Create();
            var orders = database.SaleOrders
                .AsNoTracking()
                .OrderByDescending(order => order.OrderDate)
                .Select(order => new SaleOrderListItem
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    PartnerName = order.Partner.Name,
                    TotalAmount = order.TotalAmount
                })
                .ToList();

            _orders.Clear();
            foreach (var order in orders)
            {
                _orders.Add(order);
            }

            _ordersView.Refresh();
            _selectedOrder = null;
            OrderDetailPanel.Visibility = Visibility.Collapsed;
            UpdateViewState();
        }
        catch (Exception exception)
        {
            ShowError("Die Verkaufsaufträge konnten nicht geladen werden.", exception);
        }
    }

    private bool FilterOrder(object item)
    {
        if (item is not SaleOrderListItem order)
        {
            return false;
        }

        var searchTerm = SearchBox?.Text.Trim();
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        return order.OrderNumber.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            || order.PartnerName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase);
    }

    private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _ordersView?.Refresh();
        UpdateViewState();
    }

    private void NewOrderButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SaleOrderDialog
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
            var partnerExists = database.Partners.Any(partner => partner.Id == dialog.PartnerId);
            if (!partnerExists)
            {
                transaction.Rollback();
                ShowError("Der ausgewählte Partner existiert nicht mehr.");
                return;
            }

            var requestedQuantities = dialog.Lines
                .GroupBy(line => line.ProductId)
                .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));
            var products = database.Products
                .Where(product => requestedQuantities.Keys.Contains(product.Id))
                .ToDictionary(product => product.Id);

            if (products.Count != requestedQuantities.Count)
            {
                transaction.Rollback();
                ShowError("Mindestens ein ausgewähltes Produkt existiert nicht mehr.");
                return;
            }

            var insufficientProduct = requestedQuantities
                .Select(entry => new
                {
                    Product = products[entry.Key],
                    RequestedQuantity = entry.Value
                })
                .FirstOrDefault(entry => entry.RequestedQuantity > entry.Product.StockQuantity);

            if (insufficientProduct is not null)
            {
                transaction.Rollback();
                MessageBox.Show(
                    $"Der Bestand von '{insufficientProduct.Product.Name}' reicht für die Bestellung nicht aus.",
                    AppConfig.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var order = new SaleOrder
            {
                OrderNumber = CreateOrderNumber(database),
                PartnerId = dialog.PartnerId,
                OrderDate = DateTime.Now,
                DeliveryDate = dialog.DeliveryDate,
                TotalAmount = dialog.TotalAmount,
                Lines = dialog.Lines.Select(line => new SaleOrderLine
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalAmount = line.LineTotal
                }).ToList()
            };

            database.SaleOrders.Add(order);
            foreach (var entry in requestedQuantities)
            {
                var product = products[entry.Key];
                product.StockQuantity -= entry.Value;
                database.StockMoves.Add(new StockMove
                {
                    ProductId = product.Id,
                    Quantity = entry.Value,
                    Type = StockMoveType.Out,
                    Timestamp = DateTime.Now
                });
            }

            database.SaveChanges();
            transaction.Commit();
            LoadOrders();
        }
        catch (DbUpdateException exception)
        {
            ShowError("Der Verkauf konnte nicht abgeschlossen werden.", exception);
        }
        catch (Exception exception)
        {
            ShowError("Der Verkauf konnte nicht abgeschlossen werden.", exception);
        }
    }

    private void OrderGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OrderGrid.SelectedItem is not SaleOrderListItem selectedItem)
        {
            _selectedOrder = null;
            OrderDetailPanel.Visibility = Visibility.Collapsed;
            return;
        }

        try
        {
            using var database = AppDbContext.Create();
            _selectedOrder = database.SaleOrders
                .AsNoTracking()
                .Include(order => order.Partner)
                .Include(order => order.Lines)
                .ThenInclude(line => line.Product)
                .Single(order => order.Id == selectedItem.Id);

            DetailOrderNumberText.Text = _selectedOrder.OrderNumber;
            DetailPartnerText.Text = $"Partner: {_selectedOrder.Partner.Name}";
            DetailDateText.Text = _selectedOrder.DeliveryDate is DateTime deliveryDate
                ? $"Auftrag: {_selectedOrder.OrderDate:dd.MM.yyyy HH:mm} · Lieferung: {deliveryDate:dd.MM.yyyy}"
                : $"Auftrag: {_selectedOrder.OrderDate:dd.MM.yyyy HH:mm}";
            DetailTotalText.Text = $"Gesamtsumme: {_selectedOrder.TotalAmount.ToString("N2", CultureInfo.CurrentCulture)} EUR";
            DetailLinesList.ItemsSource = _selectedOrder.Lines
                .Select(line => $"{line.Product.Name} · {line.Quantity.ToString("N2", CultureInfo.CurrentCulture)} x {line.UnitPrice.ToString("N2", CultureInfo.CurrentCulture)} EUR")
                .ToList();
            OrderDetailPanel.Visibility = Visibility.Visible;
        }
        catch (Exception exception)
        {
            _selectedOrder = null;
            OrderDetailPanel.Visibility = Visibility.Collapsed;
            ShowError("Die Auftragsdetails konnten nicht geladen werden.", exception);
        }
    }

    private void ExportCalendarButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedOrder is null)
        {
            return;
        }

        var content = CalendarExportService.CreateSaleOrderCalendar(_selectedOrder);
        OfflineExportService.SaveCalendarFile(
            Window.GetWindow(this)!,
            content,
            $"{_selectedOrder.OrderNumber}.ics");
    }

    private void ExportEmailButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_selectedOrder is null)
        {
            return;
        }

        var content = EmailDraftService.CreateOrderDraft(_selectedOrder);
        OfflineExportService.OfferEmailDraftExport(
            Window.GetWindow(this)!,
            content,
            $"{_selectedOrder.OrderNumber}_E-Mail-Entwurf.eml");
    }

    private static string CreateOrderNumber(AppDbContext database)
    {
        var datePrefix = DateTime.Now.ToString("yyyyMMdd");
        var existingNumbers = database.SaleOrders
            .Where(order => order.OrderNumber.StartsWith($"SO-{datePrefix}-"))
            .Select(order => order.OrderNumber)
            .ToList();

        var nextNumber = existingNumbers
            .Select(orderNumber => int.TryParse(orderNumber[(orderNumber.LastIndexOf('-') + 1)..], out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"SO-{datePrefix}-{nextNumber:0000}";
    }

    private void UpdateViewState()
    {
        var hasOrders = _orders.Count > 0;
        var hasVisibleOrders = _ordersView.Cast<object>().Any();
        OrderGrid.Visibility = hasOrders && hasVisibleOrders
            ? Visibility.Visible
            : Visibility.Collapsed;
        EmptyOrderState.Visibility = hasOrders && hasVisibleOrders
            ? Visibility.Collapsed
            : Visibility.Visible;
        OrderToolbar.Visibility = hasOrders ? Visibility.Visible : Visibility.Collapsed;
        OrderCountText.Text = hasOrders
            ? $"{_orders.Count} Verkaufsaufträge"
            : "Noch keine Einträge";

        if (hasOrders && !hasVisibleOrders)
        {
            OrderCountText.Text = "Keine Treffer für die aktuelle Suche";
        }
    }

    private static void ShowError(string message, Exception? exception = null)
    {
        if (exception is not null)
        {
            System.Diagnostics.Trace.TraceError("{0} {1}", message, exception);
        }

        MessageBox.Show(
            message,
            AppConfig.AppName,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
