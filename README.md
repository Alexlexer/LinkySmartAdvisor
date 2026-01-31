# LinkySmartAdvisor

LinkySmartAdvisor is a .NET-based smart energy advisor application designed to help users optimize their electricity consumption based on market prices and data from Linky smart meters.

## Features

- **Consumption Tracking**: Syncs consumption data from Enedis (Linky meters).
- **Market Price Sync**: Retrieves electricity spot prices from RTE (Réseau de Transport d'Électricité).
- **Cost Analysis**: Calculates daily electricity costs based on actual consumption and hourly market prices.
- **Smart Alerts**: Monitors upcoming high electricity prices and sends alerts via Telegram to help users reduce costs.
- **REST API**: Provides endpoints for data synchronization, analysis, and alerts.

## Tech Stack

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core** (SQL Server)
- **Telegram Bot API** (via `Telegram.Bot`)
- **Polly** (Resilience and transient fault handling)
- **XUnit** (Testing)

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Enedis API Credentials (for consumption data)
- RTE API Credentials (for market prices)
- Telegram Bot Token (for alerts)

## Configuration

Update `appsettings.json` in `Linky.Api` with your configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LinkySmartAdvisor;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "RteApi": {
    "BaseUrl": "https://opendata.rte-france.com",
    "ClientId": "YOUR_RTE_CLIENT_ID",
    "ClientSecret": "YOUR_RTE_CLIENT_SECRET"
  },
  "Telegram": {
    "BotToken": "YOUR_TELEGRAM_BOT_TOKEN"
  }
}
```

## Getting Started

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Alexlexer/LinkySmartAdvisor.git
    cd LinkySmartAdvisor
    ```

2.  **Apply Migrations:**
    Navigate to the `Linky.Api` directory and run:
    ```bash
    dotnet ef database update
    ```

3.  **Run the Application:**
    ```bash
    dotnet run --project Linky.Api
    ```

4.  **Access Swagger UI:**
    Open your browser and navigate to `https://localhost:7143/swagger` (port may vary) to explore the API.

## Tests

To run the unit and integration tests:

```bash
dotnet test
```

## License

[MIT](LICENSE)
