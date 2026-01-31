using Linky.Api.Domain.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Features.SmartAlerts;

public class AlertService(AppDbContext db)
{
    private const decimal HighPriceThreshold = 250.0m; // Порог дорогого электричества

    public async Task<List<PriceAlert>> GetPendingAlertsAsync()
    {
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        var highPrices = await db.MarketPrices
            .Where(p => p.Timestamp >= tomorrow && p.PricePerMWh >= HighPriceThreshold)
            .OrderBy(p => p.Timestamp)
            .ToListAsync();

        return highPrices.Select(p => new PriceAlert(
            p.Timestamp,
            p.PricePerMWh,
            "High",
            $"Внимание! Завтра в {p.Timestamp:HH:mm} ожидается пиковая цена: {p.PricePerMWh:N2} €/MWh. Рекомендуем снизить потребление."
        )).ToList();
    }
}