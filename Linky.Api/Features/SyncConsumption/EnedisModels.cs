namespace Linky.Api.Features.SyncConsumption;

// Response from Enedis (simplified structure for now)
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
    public string Value { get; set; } = string.Empty; // Power often comes as a string
    public DateTime Date { get; set; }                // Measurement time
}