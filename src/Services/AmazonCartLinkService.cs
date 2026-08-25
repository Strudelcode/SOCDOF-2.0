using System.Diagnostics;
using System.Net;
using SOCDOF.Data;

namespace SOCDOF.Services;

public static class AmazonCartLinkService
{
    private const string AmazonCartBaseUrl = "https://www.amazon.de/gp/aws/cart/add.html";

    public static string CreateCartLink(IEnumerable<Product> products)
    {
        var selectedProducts = products
            .Where(product => !string.IsNullOrWhiteSpace(product.SKU))
            .ToList();

        if (selectedProducts.Count == 0)
        {
            throw new ArgumentException("At least one product with a SKU is required.", nameof(products));
        }

        var parameters = selectedProducts
            .SelectMany((product, index) => new[]
            {
                $"ASIN.{index + 1}={WebUtility.UrlEncode(product.SKU.Trim())}",
                $"Quantity.{index + 1}=1"
            });

        return $"{AmazonCartBaseUrl}?{string.Join("&", parameters)}";
    }

    public static void OpenCartLink(string cartLink)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = cartLink,
            UseShellExecute = true
        });
    }
}
