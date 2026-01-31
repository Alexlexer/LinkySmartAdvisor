using Telegram.Bot;
using Telegram.Bot.Polling;

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
            pollingErrorHandler: (client, ex, ct) => {
                logger.LogError(ex, "Ошибка в Telegram Polling");
                return Task.CompletedTask;
            },
            receiverOptions: new ReceiverOptions { AllowedUpdates = [] },
            cancellationToken: stoppingToken
        );
    }
}