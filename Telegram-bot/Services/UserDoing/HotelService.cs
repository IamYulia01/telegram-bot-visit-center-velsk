using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class HotelService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;

        public HotelService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        public async Task HotelAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные гостиницы:\n\n(Здесь будет список гостиниц)\n\nЕсли вы хотите посмотреть подробную информацию о гостинице, выберите её номер:",
                replyMarkup: _keyboardService.GetHotelKeyboard(),
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

            if (currentSection == "hotel" || messageText == "Гостиницы")
            {
                if (messageText == "Гостиницы")
                {
                    _stateService.SetUserSection(chatId, "hotel");
                    await HotelAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (messageText == "К гостиницам")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вот доступные гостиницы:\n\n(Здесь будет список гостиниц)\n\nЕсли вы хотите посмотреть подробную информацию о гостинице, выберите её номер:",
                        replyMarkup: _keyboardService.GetHotelKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (int.TryParse(messageText, out int number) && number >= 1 && number <= 12)
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"Описание гостиницы {number}:\n\n(Здесь будет подробное описание)",
                        replyMarkup: _keyboardService.GetToHotelKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (messageText.ToLower() == "назад")
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