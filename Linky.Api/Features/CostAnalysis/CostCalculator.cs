using Linky.Api.Domain.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Features.CostAnalysis;

public class CostCalculator(AppDbContext db)
{
    public async Task<DailyCostReport?> CalculateForDayAsync(string prm, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        // 1. Get consumption for the day
        var consumption = await db.ConsumptionEntries
            .Where(c => c.Prm == prm && c.Timestamp >= dayStart && c.Timestamp < dayEnd)
            .OrderBy(c => c.Timestamp)
            .ToListAsync();

        // 2. Get market prices for the same period
        var prices = await db.MarketPrices
            .Where(p => p.Timestamp >= dayStart && p.Timestamp < dayEnd)
            .ToDictionaryAsync(p => p.Timestamp, p => p.PricePerMWh);

        if (!consumption.Any() || !prices.Any()) return null;

        var details = new List<HourlyCostBreakdown>();
        foreach (var entry in consumption)
        {
            // Find price for specific hour (or nearest available)
            var hourStart = new DateTime(entry.Timestamp.Year, entry.Timestamp.Month, entry.Timestamp.Day, entry.Timestamp.Hour, 0, 0, DateTimeKind.Utc);

            if (prices.TryGetValue(hourStart, out var priceMWh))
            {
                var kWh = entry.Watts / 1000m; // Convert Watts to kWh
                var priceKWh = priceMWh / 1000m; // Convert €/MWh to €/kWh
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