namespace Linky.Api.Domain;

public class MarketPrice
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; } // The hour that the price applies to
    public decimal PricePerMWh { get; set; } // Price in Euros per MWh
    public string Area { get; set; } = "France";
}