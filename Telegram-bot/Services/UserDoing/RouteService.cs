using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Services;

namespace Telegram_bot.Services
{
    public class RouteService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;

        public RouteService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task RouteAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Ваши индивидуальные маршруты: \n\n (Здесь будут маршруты, если они созданы)",
                replyMarkup: _keyboardService.GetRouteKeyboard(),
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
            if (currentSection == "route" || messageText == "Индивидуальные маршруты")
            {
                if (messageText == "Индивидуальные маршруты")
                {
                    _stateService.SetUserSection(chatId, "route");
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "создать маршрут")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите, что вы хотите добавить в маршрут: \n1. Достопримечательность\n2. Мероприятие\n3. Место общепита\n4. Гостиница\nСохранить\nВернуться",
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "достопримечательность")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вот спиок доступных достопримечеткльностей: \n\n(Здесь будет список достопримечательностей)\n\nЕсли хотите добавить достопримечательность в маршрут, выберите её номер",
                        replyMarkup: _keyboardService.GetSightKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "мероприятие")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вот спиок доступных мероприятий: \n\n(Здесь будет список мероприятий)\n\nЕсли хотите добавить мероприятие в маршрут, выберите его номер",
                        replyMarkup: _keyboardService.GetEventListKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "место общепита")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вот спиок доступных мест общепита: \n\n(Здесь будет список мест общепита)\n\nЕсли хотите добавить общепит в маршрут, выберите его номер",
                        replyMarkup: _keyboardService.GetCateringKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "гостиница")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вот спиок доступных гостиниц: \n\n(Здесь будет список гостиниц)\n\nЕсли хотите добавить гостиницу в маршрут, выберите её номер",
                        replyMarkup: _keyboardService.GetHotelKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 16)
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Объект добавлен в маршрут!",
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Маршрут добавлен!",
                        cancellationToken: cancellationToken);
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "удалить маршрут")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите маршрут, который хотите удалить:\n\n(здесь будет список названий маршрутов)",
                        replyMarkup: _keyboardService.GetDeleteRouteKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "1 маршрут" || messageText.ToLower() == "2 маршрут" || messageText.ToLower() == "3 маршрут")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вы уверены, что хотите удалить маршрут?",
                        replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Маршрут удален!",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите маршрут, который хотите удалить:\n\n(здесь будет список названий маршрутов)",
                    replyMarkup: _keyboardService.GetDeleteRouteKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите маршрут, который хотите удалить:\n\n(здесь будет список названий маршрутов)",
                        replyMarkup: _keyboardService.GetDeleteRouteKeyboard(),
                        cancellationToken: cancellationToken); return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите, что вы хотите добавить в маршрут: \n1. Достопримечательность\n2. Мероприятие\n3. Место общепита\n4. Гостиница\nСохранить\nВернуться",
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "в главное меню")
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