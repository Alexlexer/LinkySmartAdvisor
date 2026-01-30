namespace Linky.Api.Domain;

public class ConsumptionEntry
{
    public Guid Id { get; set; }

    // Точное время измерения (Enedis присылает данные каждые 30 мин)
    public DateTime Timestamp { get; set; }

    // Мощность в Ваттах
    public decimal Watts { get; set; }

    // Идентификатор счетчика (Point de Référence Mesure)
    // Во Франции это строка из 14 цифр
    public string Prm { get; set; } = string.Empty;
}