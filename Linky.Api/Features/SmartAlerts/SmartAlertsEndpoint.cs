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
                return Results.Ok(new { Message = "Prices are normal, no alerts." });
            }

            // Future Telegram/Email sending implementation
            // For now, return list of found anomalies
            return Results.Ok(alerts);
        })
        .WithTags("Alerts")
        .WithSummary("Check for upcoming high price peaks");
    }
}