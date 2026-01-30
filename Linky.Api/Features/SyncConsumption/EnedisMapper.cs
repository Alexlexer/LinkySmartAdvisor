using Linky.Api.Domain;
using Linky.Api.Features.Common;

namespace Linky.Api.Features.SyncConsumption;

public class EnedisMapper : IManualMapper<EnedisLoadCurveResponse, List<ConsumptionEntry>>
{
    public List<ConsumptionEntry> Map(EnedisLoadCurveResponse source)
    {
        return source.MeterReading.IntervalReadings.Select(reading => new ConsumptionEntry
        {
            Id = Guid.NewGuid(),
            Prm = source.MeterReading.UsagePointId,
            Timestamp = reading.Date,
            // Безопасный парсинг строки в decimal
            Watts = decimal.TryParse(reading.Value, out var watts) ? watts : 0
        }).ToList();
    }
}