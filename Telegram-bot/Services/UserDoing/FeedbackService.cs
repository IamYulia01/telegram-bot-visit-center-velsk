using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Services;

namespace Telegram_bot.Services
{
    public class FeedbackService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;

        public FeedbackService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task FeedbackAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Для отправки обратной связи отрпавьте необходимую информацию!",
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
            if (currentSection == "feedback" || messageText == "Обратная связь")
            {
                if (messageText == "Обратная связь")
                {
                    _stateService.SetUserSection(chatId, "feedback");
                    await FeedbackAsync(botClient, chatId, cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите тему сообщения",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите текст сообщения",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите ваш контактный номер",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вот письмо, которое будет отправлено: \n\n(здесь будет введенная информация)\n\nВы уверены, что хотите отправить обратную связь?",
                        replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Обратная связь отправлена!",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите действие:\n1. Достопримечательности\n2. Мероприятия\n3.Гостиницы\n4. Места общепита\n5. Сувениры\n6. Анкета\n7. Индивидуальные маршруты\n8. Обратная связь",
                        replyMarkup: _keyboardService.GetMainMenuKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Обратная связь не отправлена!",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите действие:\n1. Достопримечательности\n2. Мероприятия\n3.Гостиницы\n4. Места общепита\n5. Сувениры\n6. Анкета\n7. Индивидуальные маршруты\n8. Обратная связь",
                        replyMarkup: _keyboardService.GetMainMenuKeyboard(),
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