using System.Globalization;
using System.Net;
using System.Text;
using SOCDOF.Data;

namespace SOCDOF.Services;

public static class EmailDraftService
{
    public static string CreateOrderDraft(SaleOrder order)
    {
        var subject = $"Auftragsbestätigung {order.OrderNumber}";
        var plainText = BuildOrderPlainText(order);
        var html = BuildOrderHtml(order);
        return CreateEml(order.Partner.Email, subject, plainText, html);
    }

    public static string CreatePartnerDraft(Partner partner)
    {
        var subject = $"Nachricht von {AppConfig.AppName}";
        var plainText = $"Guten Tag {partner.Name},\r\n\r\n\r\nViele Grüße\r\n{AppConfig.AppName}";
        var html = $"<html><body><p>Guten Tag {Html(partner.Name)},</p><p></p><p>Viele Grüße<br>{Html(AppConfig.AppName)}</p></body></html>";
        return CreateEml(partner.Email, subject, plainText, html);
    }

    private static string CreateEml(string? recipient, string subject, string plainText, string html)
    {
        var boundary = $"----=_SOCDOF_{Guid.NewGuid():N}";
        var builder = new StringBuilder();
        builder.AppendLine("MIME-Version: 1.0");
        builder.AppendLine($"To: {Header(recipient ?? string.Empty)}");
        builder.AppendLine($"Subject: {Header(subject)}");
        builder.AppendLine("X-Unsent: 1");
        builder.AppendLine("Content-Type: multipart/alternative;");
        builder.AppendLine($" boundary=\"{boundary}\"");
        builder.AppendLine();
        builder.AppendLine($"--{boundary}");
        builder.AppendLine("Content-Type: text/plain; charset=utf-8");
        builder.AppendLine("Content-Transfer-Encoding: 8bit");
        builder.AppendLine();
        builder.AppendLine(plainText);
        builder.AppendLine($"--{boundary}");
        builder.AppendLine("Content-Type: text/html; charset=utf-8");
        builder.AppendLine("Content-Transfer-Encoding: 8bit");
        builder.AppendLine();
        builder.AppendLine(html);
        builder.AppendLine($"--{boundary}--");
        return builder.ToString();
    }

    private static string BuildOrderPlainText(SaleOrder order)
    {
        var lines = string.Join(
            Environment.NewLine,
            order.Lines.Select(line =>
                $"- {line.Product.Name} ({line.Product.SKU}): {line.Quantity.ToString("N2", CultureInfo.CurrentCulture)} x {line.UnitPrice.ToString("N2", CultureInfo.CurrentCulture)} EUR = {line.TotalAmount.ToString("N2", CultureInfo.CurrentCulture)} EUR"));
        var delivery = order.DeliveryDate is DateTime date
            ? $"Liefertermin: {date:dd.MM.yyyy}{Environment.NewLine}"
            : string.Empty;
        return $"Guten Tag {order.Partner.Name},{Environment.NewLine}{Environment.NewLine}vielen Dank für Ihre Bestellung. Hier ist die Auftragsbestätigung:{Environment.NewLine}{Environment.NewLine}Auftrag: {order.OrderNumber}{Environment.NewLine}Datum: {order.OrderDate:dd.MM.yyyy HH:mm}{Environment.NewLine}{delivery}{Environment.NewLine}{lines}{Environment.NewLine}{Environment.NewLine}Gesamtsumme: {order.TotalAmount.ToString("N2", CultureInfo.CurrentCulture)} EUR{Environment.NewLine}{Environment.NewLine}Viele Grüße{Environment.NewLine}{AppConfig.AppName}";
    }

    private static string BuildOrderHtml(SaleOrder order)
    {
        var rows = string.Join(
            string.Empty,
            order.Lines.Select(line => $"<tr><td>{Html(line.Product.Name)}</td><td>{Html(line.Product.SKU)}</td><td>{line.Quantity.ToString("N2", CultureInfo.CurrentCulture)}</td><td>{line.UnitPrice.ToString("N2", CultureInfo.CurrentCulture)} EUR</td><td>{line.TotalAmount.ToString("N2", CultureInfo.CurrentCulture)} EUR</td></tr>"));
        var delivery = order.DeliveryDate is DateTime date
            ? $"<p>Liefertermin: {date:dd.MM.yyyy}</p>"
            : string.Empty;
        return $"<html><body><p>Guten Tag {Html(order.Partner.Name)},</p><p>vielen Dank für Ihre Bestellung. Hier ist die Auftragsbestätigung:</p><p><strong>Auftrag:</strong> {Html(order.OrderNumber)}<br><strong>Datum:</strong> {order.OrderDate:dd.MM.yyyy HH:mm}</p>{delivery}<table border=\"1\" cellspacing=\"0\" cellpadding=\"6\"><tr><th>Produkt</th><th>SKU</th><th>Menge</th><th>Einzelpreis</th><th>Zeilensumme</th></tr>{rows}</table><p><strong>Gesamtsumme: {order.TotalAmount.ToString("N2", CultureInfo.CurrentCulture)} EUR</strong></p><p>Viele Grüße<br>{Html(AppConfig.AppName)}</p></body></html>";
    }

    private static string Header(string value)
    {
        return value.Replace("\r", string.Empty, StringComparison.Ordinal).Replace("\n", string.Empty, StringComparison.Ordinal);
    }

    private static string Html(string value)
    {
        return WebUtility.HtmlEncode(value);
    }
}
