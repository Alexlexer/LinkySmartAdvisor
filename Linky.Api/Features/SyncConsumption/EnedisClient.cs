using System.Net.Http.Headers;

namespace Linky.Api.Features.SyncConsumption;

public class EnedisClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public EnedisClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<EnedisLoadCurveResponse?> GetLoadCurveAsync(string prm, DateTime start, DateTime end)
    {
        // В реальном Enedis здесь должен быть вызов токена через IdentityModel
        // Для начала реализуем структуру запроса
        var requestUrl = $"metering_data/v5/consumption_load_curve?usage_point_id={prm}&start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}";

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<EnedisLoadCurveResponse>();
        }

        return null;
    }
}