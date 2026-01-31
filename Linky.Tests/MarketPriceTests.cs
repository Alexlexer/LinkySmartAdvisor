using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace Linky.Tests;

public class MarketPriceTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task Can_Persist_MarketPrice_To_Db()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var price = new MarketPrice
        {
            Timestamp = DateTime.UtcNow.Date.AddHours(12),
            PricePerMWh = 120.4567m
        };

        // Act
        db.MarketPrices.Add(price);
        await db.SaveChangesAsync();

        // Assert
        var saved = await db.MarketPrices.FindAsync(price.Id);
        Assert.NotNull(saved);
        Assert.Equal(120.4567m, saved.PricePerMWh);
    }
}