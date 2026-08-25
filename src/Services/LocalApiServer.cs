using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SOCDOF.Data;
using System.Diagnostics;
using System.Threading;

namespace SOCDOF.Services;

public sealed class LocalApiServer : IAsyncDisposable
{
    private WebApplication? _application;
    private int _isRunning;

    public bool IsRunning => Volatile.Read(ref _isRunning) == 1;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!AppConfig.LocalApiEnabled || _application is not null)
        {
            return;
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(LocalApiServer).Assembly.GetName().Name ?? AppConfig.AppName,
            ContentRootPath = AppContext.BaseDirectory,
            Args = Array.Empty<string>()
        });

        builder.WebHost
            .UseKestrel()
            .UseUrls(AppConfig.LocalApiUrl);

        var application = builder.Build();
        ConfigureEndpoints(application);

        try
        {
            await application.StartAsync(cancellationToken);
            _application = application;
            Interlocked.Exchange(ref _isRunning, 1);
            Trace.TraceInformation("{0} local API started at {1}.", AppConfig.AppName, AppConfig.LocalApiUrl);
        }
        catch
        {
            await application.DisposeAsync();
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var application = Interlocked.Exchange(ref _application, null);
        Interlocked.Exchange(ref _isRunning, 0);

        if (application is null)
        {
            return;
        }

        try
        {
            using var stopTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            stopTimeout.CancelAfter(TimeSpan.FromSeconds(5));
            await application.StopAsync(stopTimeout.Token);
        }
        finally
        {
            await application.DisposeAsync();
            Trace.TraceInformation("{0} local API stopped.", AppConfig.AppName);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        GC.SuppressFinalize(this);
    }

    private void ConfigureEndpoints(WebApplication application)
    {
        application.MapGet("/api/status", () => Results.Ok(new
        {
            appName = AppConfig.AppName,
            version = AppConfig.Version,
            serverStatus = IsRunning ? "running" : "starting",
            readOnly = true,
            baseUrl = AppConfig.LocalApiUrl
        }));

        application.MapGet("/api/products", () =>
        {
            try
            {
                using var database = AppDbContext.Create();
                var products = database.Products
                    .AsNoTracking()
                    .OrderBy(product => product.Name)
                    .Select(product => new ProductApiModel
                    {
                        Id = product.Id,
                        Sku = product.SKU,
                        Name = product.Name,
                        Price = product.Price,
                        StockQuantity = product.StockQuantity,
                        Unit = product.Unit
                    })
                    .ToList();

                return Results.Ok(products);
            }
            catch (Exception exception)
            {
                return CreateProblemResponse("Products could not be loaded.", exception);
            }
        });

        application.MapGet("/api/sales", () =>
        {
            try
            {
                using var database = AppDbContext.Create();
                var sales = database.SaleOrders
                    .AsNoTracking()
                    .Include(order => order.Partner)
                    .OrderByDescending(order => order.OrderDate)
                    .Select(order => new SaleApiModel
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        OrderDate = order.OrderDate,
                        PartnerId = order.PartnerId,
                        PartnerName = order.Partner.Name,
                        TotalAmount = order.TotalAmount
                    })
                    .ToList();

                return Results.Ok(sales);
            }
            catch (Exception exception)
            {
                return CreateProblemResponse("Sales could not be loaded.", exception);
            }
        });
    }

    private static IResult CreateProblemResponse(string message, Exception exception)
    {
        Trace.TraceError("{0} {1}", message, exception);
        return Results.Problem(
            title: "SOCDOF local API error",
            detail: "The requested data could not be loaded.",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    private sealed class ProductApiModel
    {
        public int Id { get; init; }
        public string Sku { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal StockQuantity { get; init; }
        public string Unit { get; init; } = string.Empty;
    }

    private sealed class SaleApiModel
    {
        public int Id { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public DateTime OrderDate { get; init; }
        public int PartnerId { get; init; }
        public string PartnerName { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
    }
}
