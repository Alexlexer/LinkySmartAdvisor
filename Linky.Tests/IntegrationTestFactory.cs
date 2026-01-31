using Linky.Api.Domain.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost; // ОБЯЗАТЕЛЬНО для ConfigureTestServices
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Linky.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // 1. Полностью вычищаем старый контекст и его опции
            var descriptors = services.Where(d =>
                d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                d.ServiceType == typeof(AppDbContext)).ToList();

            foreach (var descriptor in descriptors) services.Remove(descriptor);

            // 2. Создаем изолированный провайдер специально для InMemory
            var internalServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // 3. Регистрируем контекст с использованием этого провайдера
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb")
                       .UseInternalServiceProvider(internalServiceProvider);
            });
        });
    }
}