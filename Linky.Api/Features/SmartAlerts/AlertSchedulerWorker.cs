namespace Linky.Api.Features.SmartAlerts;

public class AlertSchedulerWorker(IServiceScopeFactory scopeFactory, ILogger<AlertSchedulerWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            // Целевое время — 15:00
            var scheduledTime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0);

            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            var delay = scheduledTime - now;
            logger.LogInformation("Следующая проверка цен запланирована на: {Time}", scheduledTime);

            await Task.Delay(delay, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var alertService = scope.ServiceProvider.GetRequiredService<AlertService>();
                var alerts = await alertService.GetPendingAlertsAsync();

                foreach (var alert in alerts)
                {
                    logger.LogWarning("ALERT: {Message}", alert.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при выполнении запланированной проверки цен");
            }
        }
    }
}