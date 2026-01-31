namespace Linky.Api.Features.SyncMarketPrices;

// Ответ для получения OAuth2 токена
public record RteTokenResponse(string access_token, int expires_in);

// Упрощенная структура ответа RTE (API: /open_api/market_price/v1/spot)
public record RteMarketPriceResponse(List<MarketPriceData> market_price);

public record MarketPriceData(List<PriceValue> values);

public record PriceValue(
    DateTime start_date, // Начало часа
    DateTime end_date,   // Конец часа
    decimal value        // Цена в €/MWh
);