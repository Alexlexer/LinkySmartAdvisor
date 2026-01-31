namespace Linky.Api.Features.SmartAlerts;

public record PriceAlert(
    DateTime Timestamp,
    decimal PricePerMWh,
    string Level, // "High", "Low", "Normal"
    string Message);