namespace Linky.Api.Domain;

public class MarketPrice
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; } // Час, к которому относится цена
    public decimal PricePerMWh { get; set; } // Цена в Евро за МВтч
    public string Area { get; set; } = "France";
}