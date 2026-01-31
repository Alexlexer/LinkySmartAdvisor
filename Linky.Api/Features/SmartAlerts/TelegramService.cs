using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Linky.Api.Features.SmartAlerts;

public class TelegramService(ITelegramBotClient botClient, ILogger<TelegramService> logger)
{
    // В идеале этот ID нужно сохранить в БД или Config, чтобы не терять при перезагрузке
    private static long? _boundChatId;

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message is not { Text: { } messageText } message) return;

        var chatId = message.Chat.Id;

        if (messageText == "/start")
        {
            _boundChatId = chatId;
            logger.LogInformation("Бот успешно привязан к ChatId: {ChatId}", chatId);

            await botClient.SendMessage(chatId,
                "✅ *Связь установлена!*\n\nЯ запомнил ваш ID. Теперь я буду присылать уведомления о высоких ценах на электричество сюда.",
                parseMode: ParseMode.Markdown);
        }
    }

    public async Task SendAlertAsync(string message)
    {
        if (_boundChatId.HasValue)
        {
            await botClient.SendMessage(_boundChatId.Value, message, parseMode: ParseMode.Markdown);
        }
        else
        {
            logger.LogWarning("Алерт не отправлен: ChatId не определен. Напишите боту /start");
        }
    }
}