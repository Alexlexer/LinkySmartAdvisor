using Linky.Api.Features.SyncMarketPrices;

using Microsoft.Extensions.DependencyInjection;

namespace Linky.Tests;

public class RteClientTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public void RteClient_Should_Be_Registered_In_DI()
    {
        using var scope = factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetService<RteClient>();
        Assert.NotNull(client);
    }
}