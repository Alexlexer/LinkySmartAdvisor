using Linky.Api.Domain;
using Linky.Api.Domain.Infrastructure;
using Linky.Api.Features.Common;
using Linky.Api.Features.SyncConsumption;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключаем БД (из нашего Пункта 2)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Стандартные инструменты OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Настройка Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddScoped<IManualMapper<EnedisLoadCurveResponse, List<ConsumptionEntry>>, EnedisMapper>();

app.UseHttpsRedirection();

// Эндпоинты будем регистрировать здесь через методы расширения для каждого слайса
// Например: app.MapSyncConsumptionEndpoints();

app.Run();