using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;
using Linky.Api.Features.SmartAlerts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Linky.Tests;

public class SmartAlertTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task Should_Identify_High_Price_Peaks()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        // 1. Add "expensive" price (300 €/MWh > 250)
        db.MarketPrices.Add(new MarketPrice
        {
            Timestamp = tomorrow.AddHours(18),
            PricePerMWh = 300m,
            Area = "France"
        });

        // 2. Add "normal" price (100 €/MWh < 250)
        db.MarketPrices.Add(new MarketPrice
        {
            Timestamp = tomorrow.AddHours(10),
            PricePerMWh = 100m,
            Area = "France"
        });

        await db.SaveChangesAsync();
        var alertService = scope.ServiceProvider.GetRequiredService<AlertService>();

        // Act
        var alerts = await alertService.GetPendingAlertsAsync();

        // Assert
        Assert.Single(alerts); // Should be only one alert
        Assert.Equal(300m, alerts[0].PricePerMWh);
        Assert.Contains("Attention!", alerts[0].Message);
    }

    [Fact]
    public void AlertWorker_Should_Be_Registered_And_Running()
    {
        // Check that HostedService is registered in the system
        var worker = factory.Services.GetServices<IHostedService>()
            .FirstOrDefault(s => s is AlertSchedulerWorker);

        Assert.NotNull(worker);
    }

}