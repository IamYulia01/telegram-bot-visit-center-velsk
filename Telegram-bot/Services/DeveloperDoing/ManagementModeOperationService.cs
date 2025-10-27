using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementModeOperationService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementModeOperationService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementModeOperationAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Управление рабочими графиками",
                replyMarkup: _keyboardService.GetOperatingModeDeveloperKeyboard(),
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
                    currentSection.StartsWith("modeOperation") ||
                    messageText == "Управление рабочими графиками" ||
                    messageText == "Рабочие графики достопримечательностей" ||
                    messageText == "Рабочие графики общепита")
            {
                if (messageText == "Управление рабочими графиками")
                {
                    _stateService.SetUserSection(chatId, "managementModeOperation");
                    await ManagementModeOperationAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (messageText == "Рабочие графики достопримечательностей")
                {
                    _stateService.SetUserSection(chatId, "modeOperationSight");
                    await ShowModeOperationActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }

                if (messageText == "Рабочие графики общепита")
                {
                    _stateService.SetUserSection(chatId, "modeOperationCatering");
                    await ShowModeOperationActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }

                if (currentSection == "modeOperationSight" || currentSection.StartsWith("modeOperationSight"))
                {
                    return await HandleModeOperationSight(botClient, chatId, messageText, currentSection, cancellationToken);
                }

                if (currentSection == "modeOperationCatering" || currentSection.StartsWith("modeOperationCatering"))
                {
                    return await HandleModeOperationCatering(botClient, chatId, messageText, currentSection, cancellationToken);
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

        private async Task ShowModeOperationActions(ITelegramBotClient botClient, long chatId, string type, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                $"Управление рабочими графиками:\n\nВыберите действие",
                replyMarkup: _keyboardService.GetDoingDeveloperKeyboard(),
                cancellationToken: cancellationToken);
        }

        private async Task<bool> HandleModeOperationSight(ITelegramBotClient botClient, long chatId, string messageText, string currentSection, CancellationToken cancellationToken)
        {
            if (currentSection == "modeOperationSight" && messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "modeOperationSightDelete");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите рабочий график достопримечательности, который хотите удалить",
                replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "modeOperationSightDelete")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить рабочий график достопримечательности №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Рабочий график достопримечательности удален",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер рабочего графика достопримечательности, который хотите удалить",
                    replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер рабочего графика достопримечательности, который хотите удалить",
                    replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "modeOperationSight");
                    await ShowModeOperationActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "modeOperationSight" && messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "modeOperationSightAdd");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите день недели рабочего графика достопримечательности",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время начала рабочего дня",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время окончания рабочего дня",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить рабочий график достопримечательности?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "modeOperationSightAdd")
            {
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Рабочий график достопримечательности добавлен",
                    cancellationToken: cancellationToken);
                    _stateService.SetUserSection(chatId, "modeOperationSight");
                    await ShowModeOperationActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "отменить")
                {
                    _stateService.SetUserSection(chatId, "modeOperationSight");
                    await ShowModeOperationActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "modeOperationSight" && messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "modeOperationSightEdit");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер рабочего графика достопримечательности, который хотите изменить",
                replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "modeOperationSightEdit")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о рабочем графике достопримечательности №{number}:" +
                    "\n1. День недели\n2. Время начала рабочего дня\n3. Время окончания рабочего дня\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditOperationModeKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "день недели")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый день недели",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время начала рабочего дня")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время начала рабочего дня",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время окончания рабочего дня")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время окончания рабочего дня",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Данные рабочего графика достопримечательности изменены",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер рабочего графика достопримечательности, который хотите изменить",
                    replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    _stateService.SetUserSection(chatId, "modeOperationSight");
                    await ShowModeOperationActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "modeOperationSight");
                    await ShowModeOperationActions(botClient, chatId, "достопримечательностей", cancellationToken);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> HandleModeOperationCatering(ITelegramBotClient botClient, long chatId, string messageText, string currentSection, CancellationToken cancellationToken)
        {
            if (currentSection == "modeOperationCatering" && messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "modeOperationCateringDelete");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите рабочий график общепита, который хотите удалить",
                replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "modeOperationCateringDelete")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить рабочий график общепита №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Рабочий график общепита удален",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер рабочего графика общепита, который хотите удалить",
                    replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер рабочего графика общепита, который хотите удалить",
                    replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "modeOperationCatering");
                    await ShowModeOperationActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "modeOperationCatering" && messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "modeOperationCateringAdd");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите день недели рабочего графика общепита",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время начала рабочего дня",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите время окончания рабочего дня",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить рабочий график общепита?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "modeOperationCateringAdd")
            {
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Рабочий график общепита добавлен",
                    cancellationToken: cancellationToken);
                    _stateService.SetUserSection(chatId, "modeOperationCatering");
                    await ShowModeOperationActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "отменить")
                {
                    _stateService.SetUserSection(chatId, "modeOperationCatering");
                    await ShowModeOperationActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
            }

            if (currentSection == "modeOperationCatering" && messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "modeOperationCateringEdit");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер рабочего графика общепита, который хотите изменить",
                replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (currentSection == "modeOperationCateringEdit")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о рабочем графике общепита №{number}:" +
                    "\n1. День недели\n2. Время начала работы\n3. Время окончания работы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditOperationModeKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "день недели")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый день недели",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время начала рабочего дня")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время начала рабочего дня",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "время окончания рабочего дня")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое время окончания рабочего дня",
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Данные рабочего графика общепита изменены",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер рабочего графика общепита, который хотите изменить",
                    replyMarkup: _keyboardService.GetModeOperationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    _stateService.SetUserSection(chatId, "modeOperationCatering");
                    await ShowModeOperationActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "modeOperationCatering");
                    await ShowModeOperationActions(botClient, chatId, "общепита", cancellationToken);
                    return true;
                }
            }

            return false;
        }
    }
}