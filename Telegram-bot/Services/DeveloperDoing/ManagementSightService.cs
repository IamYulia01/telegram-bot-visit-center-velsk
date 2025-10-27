using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementSightService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementSightService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementSightAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные достопримечательности:\n\nВыберите, что вы хотите сделать",
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
                    currentSection == "deleteSight" ||
                    currentSection == "addSight" ||
                    currentSection == "editSight")
            {
                if (messageText == "Управление достопримечательностями")
                {
                    _stateService.SetUserSection(chatId, "managementSight");
                    await ManagementSightAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (currentSection == "deleteSight" || messageText == "Удалить")
                {
                    return await HandleDeleteSight(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "addSight" || messageText == "Добавить")
                {
                    return await HandleAddSight(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "editSight" || messageText == "Изменить")
                {
                    return await HandleEditSight(botClient, chatId, messageText, cancellationToken);
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
        private async Task<bool> HandleDeleteSight(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "deleteSight");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер достопримечательности, которую хотите удалить",
                replyMarkup: _keyboardService.GetSightDeveloperKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (_stateService.GetUserSection(chatId) == "deleteSight")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 16)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить достопримечательность №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Достопримечательность удалена",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер достопримечательности, которую хотите удалить",
                    replyMarkup: _keyboardService.GetSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер достопримечательности, которую хотите удалить",
                    replyMarkup: _keyboardService.GetSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementSight");
                    await ManagementSightAsync(botClient, chatId, cancellationToken);
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> HandleAddSight(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "addSight");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите url достопримечательности",
                cancellationToken: cancellationToken);
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
                "Введите электронную почту",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите контактный номер",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите количество мест",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите описание",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите улицу расположения",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дом расположения",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить достопримечательность?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "сохранить")
            {
                await botClient.SendTextMessageAsync(
                chatId,
                $"Достопримечательность добавлена",
                cancellationToken: cancellationToken);
                _stateService.SetUserSection(chatId, "managementSight");
                await ManagementSightAsync(botClient, chatId, cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "отменить")
            {
                _stateService.SetUserSection(chatId, "managementSight");
                await ManagementSightAsync(botClient, chatId, cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<bool> HandleEditSight(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (_stateService.GetUserSection(chatId) == "editSight")
            {


                if (messageText == "Изменить")
                {
                    _stateService.SetUserSection(chatId, "editSight");
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер достопримечательности, которую хотите изменить",
                    replyMarkup: _keyboardService.GetSightDeveloperKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 16)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "url достопримечательности")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый url достопримечательности",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
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
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
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
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "электронная почта")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую электронную почту",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "контактный номер")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый контактный номер",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "количество мест")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое количество мест",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "описание")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое описание",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "улица расположения")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую улицу расположения",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дом расположения")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый дом расположения",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
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
                    "Выберите номер достопримечательности, которую хотите изменить",
                    replyMarkup: _keyboardService.GetSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Вы уверены, что хотите выйти, не сохранив изменения?",
                    replyMarkup: _keyboardService.GetSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер достопримечательности, которую хотите изменить",
                    replyMarkup: _keyboardService.GetSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о достопримечательности №{number}:" +
                    "\n1. url достопримечательности\n2. Название\n3. Тип\n4. Электронная почта\n5. Контактный номер\n6. Количество мест\n7. Описание\n8. Улица расположения\n9. Дом расположения\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSightKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementSight");
                    await ManagementSightAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                return true;
            }
            if (messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "editSight");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер достопримечательности, которую хотите изменить",
                replyMarkup: _keyboardService.GetSightKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            return false;
        }
    }
}