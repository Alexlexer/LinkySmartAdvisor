using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Features.SyncMarketPrices;

public class SyncMarketPricesHandler(RteClient rteClient, AppDbContext db)
{
    public async Task HandleAsync(DateTime start, DateTime end)
    {
        // 1. Get data from RTE API
        var response = await rteClient.GetSpotPricesAsync(start, end);
        if (response?.market_price == null) return;

        // 2. Extract all price intervals
        var priceValues = response.market_price.SelectMany(x => x.values);

        foreach (var val in priceValues)
        {
            // 3. Check if price already exists for this time (duplicate protection)
            var exists = await db.MarketPrices
                .AnyAsync(x => x.Timestamp == val.start_date);

            if (!exists)
            {
                db.MarketPrices.Add(new MarketPrice
                {
                    Id = Guid.NewGuid(),
                    Timestamp = val.start_date,
                    PricePerMWh = val.value,
                    Area = "France"
                });
            }
        }

        // 4. Save everything in a batch
        await db.SaveChangesAsync();
    }
}