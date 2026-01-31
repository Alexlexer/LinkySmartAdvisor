namespace Linky.Api.Features.SmartAlerts;

public static class SmartAlertsEndpoint
{
    public static void MapSmartAlerts(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/alerts/check", async (AlertService alertService) =>
        {
            var alerts = await alertService.GetPendingAlertsAsync();

            if (!alerts.Any())
            {
                return Results.Ok(new { Message = "Цены в норме, алертов нет." });
            }

            // Здесь в будущем будет отправка в Telegram/Email
            // А пока возвращаем список найденных аномалий
            return Results.Ok(alerts);
        })
        .WithTags("Alerts")
        .WithSummary("Check for upcoming high price peaks");
    }
}