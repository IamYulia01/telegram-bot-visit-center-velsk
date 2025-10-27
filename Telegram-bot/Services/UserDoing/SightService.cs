using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class SightService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public SightService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task SightAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные достопримечательности:\n\nЕсли вы хотите посмотреть подробную информацию о достопримечательности, выберите её номер:",
                replyMarkup: _keyboardService.GetSightKeyboard(),
                cancellationToken: cancellationToken);
        }

        public async Task<bool> TryHandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return false;

            if (message.Text is not { } messageText)
                return false;

            var chatId = message.Chat.Id;
            var currentSection = _stateService.GetUserSection(chatId);

            if (currentSection == "sight" || messageText == "Достопримечательности")
            {
                if (messageText == "Достопримечательности")
                {
                    _stateService.SetUserSection(chatId, "sight");
                    await SightAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (messageText == "Особые дни")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Особые дни достопримечательности:\n\n(Здесь будет список особых дней)",
                        replyMarkup: _keyboardService.GetToSightKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (messageText == "К достопримечательностям")
                {
                    await SightAsync(botClient, chatId, cancellationToken);

                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 16)
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"Описание достопримечательности {number}:\n\n(Здесь будет подробное описание)",
                        replyMarkup: _keyboardService.GetToSightKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите действие:\n1. Достопримечательности\n2. Мероприятия\n3.Гостиницы\n4. Места общепита\n5. Сувениры\n6. Анкета\n7. Индивидуальные маршруты\n8. Обратная связь",
                        replyMarkup: _keyboardService.GetMainMenuKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
            }
            return false;
        }
    }
}