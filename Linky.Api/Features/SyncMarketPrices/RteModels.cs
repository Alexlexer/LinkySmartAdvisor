namespace Linky.Api.Features.SyncMarketPrices;

// Response for OAuth2 token retrieval
public record RteTokenResponse(string access_token, int expires_in);

// Simplified RTE response structure (API: /open_api/market_price/v1/spot)
public record RteMarketPriceResponse(List<MarketPriceData> market_price);

public record MarketPriceData(List<PriceValue> values);

public record PriceValue(
    DateTime start_date, // Start of hour
    DateTime end_date,   // End of hour
    decimal value        // Price in €/MWh
);