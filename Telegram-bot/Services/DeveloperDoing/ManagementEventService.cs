using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementEventService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementEventService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementEventAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные мероприятия:\n\nВыберите, что вы хотите сделать",
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
                    currentSection == "deleteEvent" ||
                    currentSection == "addEvent" ||
                    currentSection == "editEvent")
            {
                if (messageText == "Управление мероприятиями")
                {
                    _stateService.SetUserSection(chatId, "managementEvent");
                    await ManagementEventAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (currentSection == "deleteEvent" || messageText == "Удалить")
                {
                    return await HandleDeleteEvent(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "addEvent" || messageText == "Добавить")
                {
                    return await HandleAddEvent(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "editEvent" || messageText == "Изменить")
                {
                    return await HandleEditEvent(botClient, chatId, messageText, cancellationToken);
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
        private async Task<bool> HandleDeleteEvent(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "deleteEvent");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер мероприятия, которое хотите удалить",
                replyMarkup: _keyboardService.GetEventListKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (_stateService.GetUserSection(chatId) == "deleteEvent")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить мероприятие №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Мероприятие удалено",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер мероприятия, которое хотите удалить",
                    replyMarkup: _keyboardService.GetEventListKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер мероприятия, которое хотите удалить",
                    replyMarkup: _keyboardService.GetEventListKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementEvent");
                    await ManagementEventAsync(botClient, chatId, cancellationToken);
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> HandleAddEvent(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "addEvent");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите название",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите тип",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дату проведения",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время начала",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите возрастное ограничение",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите улицу проведения",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дом проведения",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить мероприятие?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "сохранить")
            {
                await botClient.SendTextMessageAsync(
                chatId,
                $"Мероприятие добавлено",
                cancellationToken: cancellationToken);
                _stateService.SetUserSection(chatId, "managementEvent");
                await ManagementEventAsync(botClient, chatId, cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "отменить")
            {
                _stateService.SetUserSection(chatId, "managementEvent");
                await ManagementEventAsync(botClient, chatId, cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<bool> HandleEditEvent(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (_stateService.GetUserSection(chatId) == "editEvent")
            {


                if (messageText == "Изменить")
                {
                    _stateService.SetUserSection(chatId, "editEvent");
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер мероприятия, которое хотите изменить",
                    replyMarkup: _keyboardService.GetEventListKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 16)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "название")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое название",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "тип")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый тип",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дата проведения")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую дату проведения",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время начала")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время начала",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "возрастное ограничение")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое возрастное ограничение",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "улица проведения")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую улицу проведения",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дом проведения")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый дом проведения",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
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
                    "Выберите номер мероприятия, которое хотите изменить",
                    replyMarkup: _keyboardService.GetEventListKeyboard(),
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
                    "Выберите номер мероприятия, которое хотите изменить",
                    replyMarkup: _keyboardService.GetEventListKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о мероприятии №{number}:" +
                    "\n1. Название\n2. Тип\n3. Дата проведения\n4. Время начала\n5. Возрастное ограничение\n6. Улица проведения\n9. Дом проведения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditEventKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementEvent");
                    await ManagementEventAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                return true;
            }
            if (messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "editEvent");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер мероприятия, которое хотите изменить",
                replyMarkup: _keyboardService.GetEventListKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            return false;
        }
    }
}