using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementSpecialDayService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementSpecialDayService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementSpecialDayAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Управление особыми днями",
                replyMarkup: _keyboardService.GetSpecialDaysDeveloperKeyboard(),
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
                    currentSection.StartsWith("specialDay") ||
                    messageText == "Управление особыми днями" ||
                    messageText == "Удалить" ||
                    messageText == "Добавить" ||
                    messageText == "Изменить" ||
                    messageText.ToLower() == "сохранить" ||
                    messageText.ToLower() == "отменить" ||
                    messageText == "Особые дни достопримечательности" ||
                    messageText == "Особые дни общепита")
            {
                if (messageText == "Управление особыми днями")
                {
                    _stateService.SetUserSection(chatId, "managementSpecialDay");
                    await ManagementSpecialDayAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (messageText == "Особые дни достопримечательности")
                {
                    _stateService.SetUserSection(chatId, "specialDaySight");
                    await ShowSpecialDayActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }

                if (messageText == "Особые дни общепита")
                {
                    _stateService.SetUserSection(chatId, "specialDayCatering");
                    await ShowSpecialDayActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }

                if (currentSection == "specialDaySight" || currentSection.StartsWith("specialDaySight"))
                {
                    return await HandleSpecialDaySight(botClient, chatId, messageText, currentSection, cancellationToken);
                }

                if (currentSection == "specialDayCatering" || currentSection.StartsWith("specialDayCatering"))
                {
                    return await HandleSpecialDayCatering(botClient, chatId, messageText, currentSection, cancellationToken);
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

        private async Task ShowSpecialDayActions(ITelegramBotClient botClient, long chatId, string type, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                $"Управление особыми днями:\n\nВыберите действие",
                replyMarkup: _keyboardService.GetDoingDeveloperKeyboard(),
                cancellationToken: cancellationToken);
        }

        private async Task<bool> HandleSpecialDaySight(ITelegramBotClient botClient, long chatId, string messageText, string currentSection, CancellationToken cancellationToken)
        {
            if (currentSection == "specialDaySight" && messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "specialDaySightDelete");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите особый день достопримечательности, который хотите удалить",
                replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "specialDaySightDelete")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить особый день достопримечательности №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Особый день достопримечательности удален",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер особого дня достопримечательности, который хотите удалить",
                    replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер особого дня достопримечательности, который хотите удалить",
                    replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "specialDaySight");
                    await ShowSpecialDayActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "specialDaySight" && messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "specialDaySightAdd");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дату особого дня достопримечательности",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите статус дня",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время начала работы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время окончания работы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить особый день достопримечательности?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "specialDaySightAdd")
            {
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Особый день достопримечательности добавлен",
                    cancellationToken: cancellationToken);
                    _stateService.SetUserSection(chatId, "specialDaySight");
                    await ShowSpecialDayActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "отменить")
                {
                    _stateService.SetUserSection(chatId, "specialDaySight");
                    await ShowSpecialDayActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "specialDaySight" && messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "specialDaySightEdit");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер особого дня достопримечательности, который хотите изменить",
                replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "specialDaySightEdit")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об особом дне достопримечательности №{number}:" +
                    "\n1. Дата\n2. Статус дня\n3. Время начала работы\n4. Время окончания работы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дата")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую дату",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "статус дня")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый статус дня",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время начала работы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время начала работы",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время окончания работы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время окончания работы",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Данные особого дня достопримечательности изменены",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер особого дня достопримечательности, который хотите изменить",
                    replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    _stateService.SetUserSection(chatId, "specialDaySight");
                    await ShowSpecialDayActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "specialDaySight");
                    await ShowSpecialDayActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> HandleSpecialDayCatering(ITelegramBotClient botClient, long chatId, string messageText, string currentSection, CancellationToken cancellationToken)
        {
            if (currentSection == "specialDayCatering" && messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "specialDayCateringDelete");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите особый день общепита, который хотите удалить",
                replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "specialDayCateringDelete")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить особый день общепита №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Особый день общепита удален",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер особого дня общепита, который хотите удалить",
                    replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер особого дня общепита, который хотите удалить",
                    replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "specialDayCatering");
                    await ShowSpecialDayActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "specialDayCatering" && messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "specialDayCateringAdd");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дату особого дня общепита",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите статус дня",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время начала работы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время окончания работы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить особый день общепита?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "specialDayCateringAdd")
            {
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Особый день общепита добавлен",
                    cancellationToken: cancellationToken);
                    _stateService.SetUserSection(chatId, "specialDayCatering");
                    await ShowSpecialDayActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "отменить")
                {
                    _stateService.SetUserSection(chatId, "specialDayCatering");
                    await ShowSpecialDayActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "specialDayCatering" && messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "specialDayCateringEdit");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер особого дня общепита, который хотите изменить",
                replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "specialDayCateringEdit")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об особом дне достопримечательности №{number}:" +
                    "\n1. Дата\n2. Статус дня\n3. Время начала работы\n4. Время окончания работы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дата")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую дату",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "статус дня")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый статус дня",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время начала работы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время начала работы",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время окончания работы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время окончания работы",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Данные особого дня общепита изменены",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер особого дня общепита, который хотите изменить",
                    replyMarkup: _keyboardService.GetSpecialDayKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    _stateService.SetUserSection(chatId, "specialDayCatering");
                    await ShowSpecialDayActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "specialDayCatering");
                    await ShowSpecialDayActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
            }

            return false;
        }
    }
}