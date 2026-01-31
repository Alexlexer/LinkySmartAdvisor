using System.Net.Http.Json;

using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;
using Linky.Api.Features.CostAnalysis;

using Microsoft.Extensions.DependencyInjection;

namespace Linky.Tests;

public class CostAnalysisTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task Should_Calculate_Daily_Cost_Correctly()
    {
        // 1. Prepare data (Arrange)
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var testDate = new DateTime(2026, 01, 30, 10, 0, 0, DateTimeKind.Utc);
        var prm = "123456789";

        // Add 1 kW (1000 W) of consumption
        db.ConsumptionEntries.Add(new ConsumptionEntry { Prm = prm, Timestamp = testDate, Watts = 1000 });

        // Add price 200 €/MWh (this is 0.20 €/kWh)
        db.MarketPrices.Add(new MarketPrice { Timestamp = testDate, PricePerMWh = 200m, Area = "France" });

        await db.SaveChangesAsync();

        var client = factory.CreateClient();

        // 2. Action (Act)
        var response = await client.GetAsync($"/api/analysis/daily?prm={prm}&date={testDate:yyyy-MM-dd}");

        // 3. Check (Assert)
        response.EnsureSuccessStatusCode();
        var report = await response.Content.ReadFromJsonAsync<DailyCostReport>();

        Assert.NotNull(report);
        Assert.Equal(1.0m, report.TotalKWh); // 1000W = 1kWh
        Assert.Equal(0.20m, report.TotalCostEuro); // 1kWh * 0.20€
    }
}