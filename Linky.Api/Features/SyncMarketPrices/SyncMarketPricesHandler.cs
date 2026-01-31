using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Features.SyncMarketPrices;

public class SyncMarketPricesHandler(RteClient rteClient, AppDbContext db)
{
    public async Task HandleAsync(DateTime start, DateTime end)
    {
        // 1. Получаем данные из API RTE
        var response = await rteClient.GetSpotPricesAsync(start, end);
        if (response?.market_price == null) return;

        // 2. Вытягиваем все интервалы цен
        var priceValues = response.market_price.SelectMany(x => x.values);

        foreach (var val in priceValues)
        {
            // 3. Проверяем, нет ли уже цены на это время (защита от дублей)
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

        // 4. Сохраняем всё пачкой
        await db.SaveChangesAsync();
    }
}