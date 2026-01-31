using Linky.Api.Features.CostAnalysis;

public static class CostAnalysisEndpoint
{
    public static void MapCostAnalysis(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/analysis/daily", async (string prm, DateTime date, CostCalculator calculator) =>
        {
            var report = await calculator.CalculateForDayAsync(prm, date);
            return report != null ? Results.Ok(report) : Results.NotFound("No data found for this period.");
        })
        .WithTags("Analysis");
    }
}