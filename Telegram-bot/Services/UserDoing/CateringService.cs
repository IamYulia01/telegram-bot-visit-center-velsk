using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class CateringService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;

        public CateringService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        public async Task CateringAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные места общепита:\n\n(Здесь будет список мест общепита)\n\nЕсли вы хотите посмотреть подробную информацию об общепите, выберите его номер:",
                replyMarkup: _keyboardService.GetCateringKeyboard(),
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

            if (currentSection == "catering" || messageText == "Места общепита")
            {
                if (messageText == "Места общепита")
                {
                    _stateService.SetUserSection(chatId, "catering");

                    await CateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (messageText == "Особые дни")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Особые дни общепита:\n\n(Здесь будет список особых дней)",
                        replyMarkup: _keyboardService.GetToCateringKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (messageText == "К местам общепита")
                {
                    await CateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (int.TryParse(messageText, out int number) && number >= 1 && number <= 16)
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"Описание общепита {number}:\n\n(Здесь будет подробное описание)",
                        replyMarkup: _keyboardService.GetToCateringKeyboard(),
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