using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Linky.Api.Features.SmartAlerts;

public class TelegramService(ITelegramBotClient botClient, ILogger<TelegramService> logger)
{
    // Ideally this ID should be saved to DB or Config to avoid loss on restart
    private static long? _boundChatId;

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message is not { Text: { } messageText } message) return;

        var chatId = message.Chat.Id;

        if (messageText == "/start")
        {
            _boundChatId = chatId;
            logger.LogInformation("Bot successfully bound to ChatId: {ChatId}", chatId);

            await botClient.SendMessage(chatId,
                "✅ *Connection established!*\n\nI have saved your ID. Now I will send high electricity price notifications here.",
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
            logger.LogWarning("Alert not sent: ChatId not defined. Message /start to the bot");
        }
    }
}