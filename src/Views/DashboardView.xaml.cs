using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using SOCDOF.Data;

namespace SOCDOF.Views;

public sealed class DashboardActivity
{
    public DateTime Timestamp { get; init; }
    public string ActivityType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ValueDisplay { get; init; } = string.Empty;
}

public partial class DashboardView : UserControl
{
    private readonly ObservableCollection<DashboardActivity> _activities = new();

    public DashboardView()
    {
        InitializeComponent();
        ActivityGrid.ItemsSource = _activities;
        Loaded += DashboardView_OnLoaded;
    }

    private void DashboardView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= DashboardView_OnLoaded;
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        try
        {
            using var database = AppDbContext.Create();
            var partnerCount = database.Partners.Count();
            var productCount = database.Products.Count();
            var lowStockCount = database.Products.Count(product => product.StockQuantity < 5m);
            var orderCount = database.SaleOrders.Count();
            var revenue = database.SaleOrders.Sum(order => (decimal?)order.TotalAmount) ?? 0m;

            PartnerCountText.Text = partnerCount.ToString(CultureInfo.CurrentCulture);
            ProductCountText.Text = productCount.ToString(CultureInfo.CurrentCulture);
            LowStockText.Text = $"{lowStockCount} mit niedrigem Bestand";
            OrderCountText.Text = orderCount.ToString(CultureInfo.CurrentCulture);
            RevenueText.Text = $"{revenue.ToString("N2", CultureInfo.CurrentCulture)} EUR Umsatz";
            LastUpdatedText.Text = $"Zuletzt aktualisiert: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)}";

            var recentOrders = database.SaleOrders
                .AsNoTracking()
                .Include(order => order.Partner)
                .OrderByDescending(order => order.OrderDate)
                .Take(5)
                .ToList()
                .Select(order => new DashboardActivity
                {
                    Timestamp = order.OrderDate,
                    ActivityType = "Verkauf",
                    Description = $"{order.OrderNumber} · {order.Partner.Name}",
                    ValueDisplay = $"{order.TotalAmount.ToString("N2", CultureInfo.CurrentCulture)} EUR"
                });

            var recentMoves = database.StockMoves
                .AsNoTracking()
                .Include(move => move.Product)
                .OrderByDescending(move => move.Timestamp)
                .Take(5)
                .ToList()
                .Select(move => new DashboardActivity
                {
                    Timestamp = move.Timestamp,
                    ActivityType = move.Type == StockMoveType.In ? "Wareneingang" : "Warenausgang",
                    Description = $"{move.Product.SKU} · {move.Product.Name}",
                    ValueDisplay = $"{(move.Type == StockMoveType.In ? "+" : "-")}{move.Quantity.ToString("N2", CultureInfo.CurrentCulture)} {move.Product.Unit}"
                });

            var activities = recentOrders
                .Concat(recentMoves)
                .OrderByDescending(activity => activity.Timestamp)
                .Take(5)
                .ToList();

            _activities.Clear();
            foreach (var activity in activities)
            {
                _activities.Add(activity);
            }

            ActivityGrid.Visibility = _activities.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyActivityState.Visibility = _activities.Count > 0
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Trace.TraceError("SOCDOF dashboard loading failed: {0}", exception);
            MessageBox.Show(
                "Die Dashboard-Daten konnten nicht geladen werden.",
                AppConfig.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void NewPartnerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new PartnerDialog
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
            database.Partners.Add(dialog.ResultPartner);
            database.SaveChanges();
            LoadDashboard();
        }
        catch (Exception exception)
        {
            ShowActionError("Der Partner konnte nicht angelegt werden.", exception);
        }
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
            LoadDashboard();
        }
        catch (Exception exception)
        {
            ShowActionError("Das Produkt konnte nicht angelegt werden.", exception);
        }
    }

    private void NewSaleButton_OnClick(object sender, RoutedEventArgs e)
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
                ShowActionError("Der ausgewählte Partner existiert nicht mehr.");
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
                ShowActionError("Mindestens ein ausgewähltes Produkt existiert nicht mehr.");
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
            LoadDashboard();
        }
        catch (Exception exception)
        {
            ShowActionError("Der Verkauf konnte nicht abgeschlossen werden.", exception);
        }
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

    private static void ShowActionError(string message, Exception? exception = null)
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
