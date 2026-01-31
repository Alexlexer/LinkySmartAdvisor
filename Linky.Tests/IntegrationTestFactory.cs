using Linky.Api.Domain.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost; // MANDATORY for ConfigureTestServices
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Linky.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // 1. Completely clear old context and its options
            var descriptors = services.Where(d =>
                d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                d.ServiceType == typeof(AppDbContext)).ToList();

            foreach (var descriptor in descriptors) services.Remove(descriptor);

            // 2. Create isolated provider specifically for InMemory
            var internalServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // 3. Register context using this provider
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb")
                       .UseInternalServiceProvider(internalServiceProvider);
            });
        });
    }
}