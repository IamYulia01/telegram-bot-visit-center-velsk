using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementCateringService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementCateringService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementCateringAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные места общепита:\n\nВыберите, что вы хотите сделать",
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
                    currentSection == "deleteCatering" ||
                    currentSection == "addCatering" ||
                    currentSection == "editCatering")
            {
                if (messageText == "Управление местами общепита")
                {
                    _stateService.SetUserSection(chatId, "managementCatering");
                    await ManagementCateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (currentSection == "deleteCatering" || messageText == "Удалить")
                {
                    return await HandleDeleteCatering(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "addCatering" || messageText == "Добавить")
                {
                    return await HandleAddCatering(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "editCatering" || messageText == "Изменить")
                {
                    return await HandleEditCatering(botClient, chatId, messageText, cancellationToken);
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
        private async Task<bool> HandleDeleteCatering(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "deleteCatering");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер общепита, который хотите удалить",
                replyMarkup: _keyboardService.GetCateringKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (_stateService.GetUserSection(chatId) == "deleteCatering")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить место общепита №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Место общепита удалено",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер общепита, который хотите удалить",
                    replyMarkup: _keyboardService.GetCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер общепита, который хотите удалить",
                    replyMarkup: _keyboardService.GetCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementCatering");
                    await ManagementCateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> HandleAddCatering(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "addCatering");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите url общепита",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите название",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите категорию",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите контактный номер",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите улицу общепита",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дом общепита",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить место общепита?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "сохранить")
            {
                await botClient.SendTextMessageAsync(
                chatId,
                $"Место общепита добавлено",
                cancellationToken: cancellationToken);
                _stateService.SetUserSection(chatId, "managementCatering");
                await ManagementCateringAsync(botClient, chatId, cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "отменить")
            {
                _stateService.SetUserSection(chatId, "managementCatering");
                await ManagementCateringAsync(botClient, chatId, cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<bool> HandleEditCatering(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (_stateService.GetUserSection(chatId) == "editCatering")
            {


                if (messageText == "Изменить")
                {
                    _stateService.SetUserSection(chatId, "editCatering");
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер общепита, который хотите изменить",
                    replyMarkup: _keyboardService.GetCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 12)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "url общепита")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый url общепита",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
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
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "категория")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую категорию",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
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
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "улица общепита")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую улицу общепита",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дом общепита")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый дом общепита",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
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
                    "Выберите номер общепита, который хотите изменить",
                    replyMarkup: _keyboardService.GetCateringKeyboard(),
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
                    "Выберите номер общепита, который хотите изменить",
                    replyMarkup: _keyboardService.GetCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации об общепите №{number}:" +
                    "\n1. url общепита\n2. Название\n3. Категория\n4.Контактный номер\n5. Улица общепита\n6. Дом общепита\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditCateringKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementCatering");
                    await ManagementCateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                return true;
            }
            if (messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "editCatering");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер общепита, который хотите изменить",
                replyMarkup: _keyboardService.GetCateringKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            return false;
        }
    }
}