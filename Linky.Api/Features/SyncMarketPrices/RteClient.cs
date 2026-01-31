using System.Net.Http.Headers;
using System.Text;

namespace Linky.Api.Features.SyncMarketPrices;

public class RteClient(HttpClient httpClient, IConfiguration config)
{
    private string? _accessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public async Task<RteMarketPriceResponse?> GetSpotPricesAsync(DateTime start, DateTime end)
    {
        await EnsureAuthenticatedAsync();

        // Формат ISO8601, который ожидает RTE
        var startStr = start.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var endStr = end.ToString("yyyy-MM-ddTHH:mm:sszzz");

        var url = $"/open_api/market_price/v1/spot?start_date={Uri.EscapeDataString(startStr)}&end_date={Uri.EscapeDataString(endStr)}";

        return await httpClient.GetFromJsonAsync<RteMarketPriceResponse>(url);
    }

    private async Task EnsureAuthenticatedAsync()
    {
        // Если токен еще живет (с запасом в 30 секунд), используем его
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry.AddSeconds(-30))
            return;

        var clientId = config["RteApi:ClientId"];
        var clientSecret = config["RteApi:ClientSecret"];

        // RTE требует Basic Auth для получения токена: Base64(ID:Secret)
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "/token/oauth/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tokenData = await response.Content.ReadFromJsonAsync<RteTokenResponse>();

        _accessToken = tokenData?.access_token ?? throw new Exception("Failed to get RTE token");
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.expires_in);

        // Устанавливаем Bearer токен для основного API
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }
}