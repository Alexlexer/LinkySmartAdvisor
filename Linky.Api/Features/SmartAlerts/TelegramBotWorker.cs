using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Linky.Api.Features.SmartAlerts;

public class TelegramBotWorker(ITelegramBotClient botClient, IServiceProvider serviceProvider, ILogger<TelegramBotWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Запуск прослушивания сообщений Telegram...");

        await botClient.ReceiveAsync(
            updateHandler: async (client, update, ct) =>
            {
                using var scope = serviceProvider.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<TelegramService>();
                await svc.HandleUpdateAsync(update);
            },
            errorHandler: (client, ex, ct) => {
                logger.LogError(ex, "Ошибка в Telegram Polling");
                return Task.CompletedTask;
            },
            receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cancellationToken: stoppingToken
        );
    }
}