namespace Linky.Api.Features.SyncConsumption;

// Ответ от Enedis (упрощенная структура для начала)
public class EnedisLoadCurveResponse
{
    public MeterReading MeterReading { get; set; } = new();
}

public class MeterReading
{
    public string UsagePointId { get; set; } = string.Empty;
    public List<IntervalReading> IntervalReadings { get; set; } = new();
}

public class IntervalReading
{
    public string Value { get; set; } = string.Empty; // Мощность часто приходит строкой
    public DateTime Date { get; set; }                // Время замера
}