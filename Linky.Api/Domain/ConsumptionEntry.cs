namespace Linky.Api.Domain;

public class ConsumptionEntry
{
    public Guid Id { get; set; }

    // Exact measurement time (Enedis sends data every 30 minutes)
    public DateTime Timestamp { get; set; }

    // Power in Watts
    public decimal Watts { get; set; }

    // Meter identifier (Point de Référence Mesure)
    // In France, this is a 14-digit string
    public string Prm { get; set; } = string.Empty;
}