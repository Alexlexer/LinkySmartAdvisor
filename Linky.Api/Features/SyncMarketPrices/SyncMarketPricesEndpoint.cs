using Microsoft.AspNetCore.Mvc;

namespace Linky.Api.Features.SyncMarketPrices;

public static class SyncMarketPricesEndpoint
{
    public static void MapSyncMarketPrices(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/market-prices/sync", async (
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            SyncMarketPricesHandler handler) =>
        {
            // Minor validation: RTE usually does not return data for more than 30 days at a time
            if ((end - start).TotalDays > 31)
            {
                return Results.BadRequest("Period cannot exceed 31 days.");
            }

            await handler.HandleAsync(start, end);
            return Results.Ok(new { Message = "Sync completed successfully" });
        })
        .WithTags("MarketPrices")
        .WithSummary("Sync spot prices from RTE API");
    }
}