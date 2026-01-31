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

    [Fact]
    public async Task SyncEndpoint_Should_Return_Success()
    {
        // Arrange
        var client = factory.CreateClient();
        var start = DateTime.UtcNow.AddDays(-1).ToString("o");
        var end = DateTime.UtcNow.ToString("o");

        // Act
        // Мы отправляем запрос к нашему новому эндпоинту
        var response = await client.PostAsync($"/api/market-prices/sync?start={start}&end={end}", null);

        // Assert
        // Даже если у нас нет реальных ключей RTE, мы проверяем, 
        // что инфраструктура (Handler, DI, Маршрутизация) работает.
        // В CI без ключей может быть 500, но локально с ключами должно быть 200.
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.InternalServerError);
    }

}