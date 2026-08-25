using System.Globalization;
using System.Text;
using SOCDOF.Data;

namespace SOCDOF.Services;

public static class CalendarExportService
{
    public static string CreateSaleOrderCalendar(SaleOrder order)
    {
        var events = new StringBuilder();
        AppendTimedEvent(
            events,
            $"{order.OrderNumber}-sale",
            "Verkaufsauftrag",
            $"{order.OrderNumber} - {order.Partner.Name}",
            order.OrderDate,
            BuildOrderDescription(order));

        if (order.DeliveryDate is DateTime deliveryDate)
        {
            AppendAllDayEvent(
                events,
                $"{order.OrderNumber}-delivery",
                "Liefertermin",
                $"{order.OrderNumber} - {order.Partner.Name}",
                deliveryDate.Date,
                BuildOrderDescription(order));
        }

        return $"BEGIN:VCALENDAR\r\nVERSION:2.0\r\nPRODID:-//{EscapeText(AppConfig.AppName)}//SOCDOF//EN\r\nCALSCALE:GREGORIAN\r\n{events}END:VCALENDAR\r\n";
    }

    private static void AppendTimedEvent(
        StringBuilder calendar,
        string identifier,
        string category,
        string summary,
        DateTime start,
        string description)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = startUtc.AddHours(1);
        calendar.AppendLine("BEGIN:VEVENT");
        calendar.AppendLine($"UID:{EscapeText(identifier)}@{EscapeText(AppConfig.AppName)}");
        calendar.AppendLine($"DTSTAMP:{FormatUtc(DateTime.UtcNow)}");
        calendar.AppendLine($"DTSTART:{FormatUtc(startUtc)}");
        calendar.AppendLine($"DTEND:{FormatUtc(endUtc)}");
        calendar.AppendLine($"SUMMARY:{EscapeText(category)}: {EscapeText(summary)}");
        calendar.AppendLine($"DESCRIPTION:{EscapeText(description)}");
        calendar.AppendLine("END:VEVENT");
    }

    private static void AppendAllDayEvent(
        StringBuilder calendar,
        string identifier,
        string category,
        string summary,
        DateTime date,
        string description)
    {
        calendar.AppendLine("BEGIN:VEVENT");
        calendar.AppendLine($"UID:{EscapeText(identifier)}@{EscapeText(AppConfig.AppName)}");
        calendar.AppendLine($"DTSTAMP:{FormatUtc(DateTime.UtcNow)}");
        calendar.AppendLine($"DTSTART;VALUE=DATE:{date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}");
        calendar.AppendLine($"DTEND;VALUE=DATE:{date.AddDays(1).ToString("yyyyMMdd", CultureInfo.InvariantCulture)}");
        calendar.AppendLine($"SUMMARY:{EscapeText(category)}: {EscapeText(summary)}");
        calendar.AppendLine($"DESCRIPTION:{EscapeText(description)}");
        calendar.AppendLine("END:VEVENT");
    }

    private static string BuildOrderDescription(SaleOrder order)
    {
        var lines = string.Join(
            "\\n",
            order.Lines.Select(line =>
                $"{line.Product.SKU} {line.Product.Name}: {line.Quantity.ToString("N2", CultureInfo.InvariantCulture)} x {line.UnitPrice.ToString("N2", CultureInfo.InvariantCulture)} EUR"));
        return $"Partner: {order.Partner.Name}\\nGesamtsumme: {order.TotalAmount.ToString("N2", CultureInfo.InvariantCulture)} EUR\\n{lines}";
    }

    private static string FormatUtc(DateTime value)
    {
        return value.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
    }

    private static string EscapeText(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace(";", "\\;", StringComparison.Ordinal)
            .Replace(",", "\\,", StringComparison.Ordinal)
            .Replace("\r\n", "\\n", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\r", "\\n", StringComparison.Ordinal);
    }
}
