using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class EventService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;

        public EventService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task EventAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные мероприятия: \n\n (Здесь будет список мероприятий)",
                replyMarkup: _keyboardService.GetBackKeyboard(),
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
            if (currentSection == "event" || messageText == "Мероприятия")
            {
                if (messageText == "Мероприятия")
                {
                    _stateService.SetUserSection(chatId, "event");
                    await EventAsync(botClient, chatId, cancellationToken);
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