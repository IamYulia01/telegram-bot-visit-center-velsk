using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementTicketService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementTicketService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementTicketAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные билеты:\n\nВыберите, что вы хотите сделать",
                replyMarkup: _keyboardService.GetDoingDeveloperKeyboard(),
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

            if (currentSection.StartsWith("management") ||
                    currentSection == "deleteTicket" ||
                    currentSection == "addTicket" ||
                    currentSection == "editTicket")
            {
                if (messageText == "Управление билетами")
                {
                    _stateService.SetUserSection(chatId, "managementTicket");
                    await ManagementTicketAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (currentSection == "deleteTicket" || messageText == "Удалить")
                {
                    return await HandleDeleteTicket(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "addTicket" || messageText == "Добавить")
                {
                    return await HandleAddTicket(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "editTicket" || messageText == "Изменить")
                {
                    return await HandleEditTicket(botClient, chatId, messageText, cancellationToken);
                }

                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "management");
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите действие:\n1. Управление достопримечательностями\n2. Управление мероприятиями\n3. Управление гостиницами\n4. Управление местами общепита\n5. Управление сувенирами\n6. Управление особыми днями\n7. Управление рабочими графиками\n8. Управление билетами\n9. Выйти",
                        replyMarkup: _keyboardService.GetMainMenuDeveloperKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
            }
            return false;
        }
        private async Task<bool> HandleDeleteTicket(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "deleteTicket");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите билет, который хотите удалить",
                replyMarkup: _keyboardService.GetTicketKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (_stateService.GetUserSection(chatId) == "deleteTicket")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить билет №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Билет удалено",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер билета, который хотите удалить",
                    replyMarkup: _keyboardService.GetTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер билета, который хотите удалить",
                    replyMarkup: _keyboardService.GetTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementTicket");
                    await ManagementTicketAsync(botClient, chatId, cancellationToken);
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> HandleAddTicket(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "addTicket");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите мероприятие",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите минимальный возраст",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите максимальный возраст",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите цену",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить билет?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "сохранить")
            {
                await botClient.SendTextMessageAsync(
                chatId,
                $"Билет добавлен",
                cancellationToken: cancellationToken);
                _stateService.SetUserSection(chatId, "managementTicket");
                await ManagementTicketAsync(botClient, chatId, cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "отменить")
            {
                _stateService.SetUserSection(chatId, "managementTicket");
                await ManagementTicketAsync(botClient, chatId, cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<bool> HandleEditTicket(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (_stateService.GetUserSection(chatId) == "editTicket")
            {


                if (messageText == "Изменить")
                {
                    _stateService.SetUserSection(chatId, "editTicket");
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер билета, который хотите изменить",
                    replyMarkup: _keyboardService.GetTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 12)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о билете №{number}:" +
                    "\n1. Мероприятие\n2. Минимальный возраст\n3. Максимальный возраст\n4. Цена\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "мероприятие")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое мероприятие",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о билете №{number}:" +
                    "\n1. Мероприятие\n2. Минимальный возраст\n3. Максимальный возраст\n4. Цена\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "минимальный возраст")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый минимальный возраст",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о билете №{number}:" +
                    "\n1. Мероприятие\n2. Минимальный возраст\n3. Максимальный возраст\n4. Цена\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "максимальный возраст")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый максимальный возраст",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о билете №{number}:" +
                    "\n1. Мероприятие\n2. Минимальный возраст\n3. Максимальный возраст\n4. Цена\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "цена")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую цену",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о билете №{number}:" +
                    "\n1. Мероприятие\n2. Минимальный возраст\n3. Максимальный возраст\n4. Цена\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Данные изменены",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер билета, который хотите изменить",
                    replyMarkup: _keyboardService.GetTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Вы уверены, что хотите выйти, не сохранив изменения?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер билета, который хотите изменить",
                    replyMarkup: _keyboardService.GetTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о билете №{number}:" +
                    "\n1. Мероприятие\n2. Минимальный возраст\n3. Максимальный возраст\n4. Цена\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditTicketKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementTicket");
                    await ManagementTicketAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                return true;
            }
            if (messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "editTicket");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер билета, который хотите изменить",
                replyMarkup: _keyboardService.GetTicketKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            return false;
        }
    }
}