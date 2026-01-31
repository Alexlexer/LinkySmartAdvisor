using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;
using Linky.Api.Features.Common;
using Linky.Api.Features.SyncConsumption;
using Linky.Api.Features.SyncMarketPrices;

using Microsoft.EntityFrameworkCore;

using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключаем БД (из нашего Пункта 2)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Стандартные инструменты OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.AddHttpClient<EnedisClient>(client =>
{
    client.BaseAddress = new Uri("https://ext.api.enedis.fr/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(retryPolicy);

builder.Services.AddHttpClient<RteClient>(client =>
{
    // По умолчанию используем PROD адрес RTE
    var baseUrl = builder.Configuration["RteApi:BaseUrl"] ?? "https://opendata.rte-france.com";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IManualMapper<EnedisLoadCurveResponse, List<ConsumptionEntry>>, EnedisMapper>();

builder.Services.AddScoped<SyncMarketPricesHandler>();

var app = builder.Build();

// 3. Настройка Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Эндпоинты будем регистрировать здесь через методы расширения для каждого слайса
// Например: app.MapSyncConsumptionEndpoints();
app.MapSyncMarketPrices();

app.Run();