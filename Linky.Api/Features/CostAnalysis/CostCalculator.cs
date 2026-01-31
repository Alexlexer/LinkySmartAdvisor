using Linky.Api.Domain.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Features.CostAnalysis;

public class CostCalculator(AppDbContext db)
{
    public async Task<DailyCostReport?> CalculateForDayAsync(string prm, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        // 1. Получаем потребление за день
        var consumption = await db.ConsumptionEntries
            .Where(c => c.Prm == prm && c.Timestamp >= dayStart && c.Timestamp < dayEnd)
            .OrderBy(c => c.Timestamp)
            .ToListAsync();

        // 2. Получаем рыночные цены за тот же период
        var prices = await db.MarketPrices
            .Where(p => p.Timestamp >= dayStart && p.Timestamp < dayEnd)
            .ToDictionaryAsync(p => p.Timestamp, p => p.PricePerMWh);

        if (!consumption.Any() || !prices.Any()) return null;

        var details = new List<HourlyCostBreakdown>();
        foreach (var entry in consumption)
        {
            // Находим цену для конкретного часа (или ближайшую доступную)
            var hourStart = new DateTime(entry.Timestamp.Year, entry.Timestamp.Month, entry.Timestamp.Day, entry.Timestamp.Hour, 0, 0, DateTimeKind.Utc);

            if (prices.TryGetValue(hourStart, out var priceMWh))
            {
                var kWh = entry.Watts / 1000m; // Переводим Ватты в кВтч
                var priceKWh = priceMWh / 1000m; // Переводим €/MWh в €/kWh
                var cost = kWh * priceKWh;

                details.Add(new HourlyCostBreakdown(entry.Timestamp, kWh, priceKWh, cost));
            }
        }

        return new DailyCostReport(
            dayStart,
            details.Sum(x => x.KWh),
            details.Sum(x => x.CostEuro),
            details);
    }
}