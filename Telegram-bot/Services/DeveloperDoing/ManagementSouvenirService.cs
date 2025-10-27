using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementSouvenirService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementSouvenirService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementSouvenirAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные сувениры:\n\nВыберите, что вы хотите сделать",
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
                    currentSection == "deleteSouvenir" ||
                    currentSection == "addSouvenir" ||
                    currentSection == "editSouvenir")
            {
                if (messageText == "Управление сувенирами")
                {
                    _stateService.SetUserSection(chatId, "managementSouvenir");
                    await ManagementSouvenirAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (currentSection == "deleteSouvenir" || messageText == "Удалить")
                {
                    return await HandleDeleteSouvenir(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "addSouvenir" || messageText == "Добавить")
                {
                    return await HandleAddSouvenir(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "editSouvenir" || messageText == "Изменить")
                {
                    return await HandleEditSouvenir(botClient, chatId, messageText, cancellationToken);
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
        private async Task<bool> HandleDeleteSouvenir(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "deleteSouvenir");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите сувенира, который хотите удалить",
                replyMarkup: _keyboardService.GetSouvenirKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (_stateService.GetUserSection(chatId) == "deleteSouvenir")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить сувенир №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Сувенир удалено",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер сувенира, который хотите удалить",
                    replyMarkup: _keyboardService.GetSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер сувенира, который хотите удалить",
                    replyMarkup: _keyboardService.GetSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementSouvenir");
                    await ManagementSouvenirAsync(botClient, chatId, cancellationToken);
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> HandleAddSouvenir(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "addSouvenir");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите название сувенира",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите продукт",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите вкус",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите вес",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить сувенир?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "сохранить")
            {
                await botClient.SendTextMessageAsync(
                chatId,
                $"Сувенир добавлен",
                cancellationToken: cancellationToken);
                _stateService.SetUserSection(chatId, "managementSouvenir");
                await ManagementSouvenirAsync(botClient, chatId, cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "отменить")
            {
                _stateService.SetUserSection(chatId, "managementSouvenir");
                await ManagementSouvenirAsync(botClient, chatId, cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<bool> HandleEditSouvenir(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (_stateService.GetUserSection(chatId) == "editSouvenir")
            {


                if (messageText == "Изменить")
                {
                    _stateService.SetUserSection(chatId, "editSouvenir");
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер сувенира, который хотите изменить",
                    replyMarkup: _keyboardService.GetSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 12)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о сувенире №{number}:" +
                    "\n1. Название сувенира\n2. Продукт\n3. Вкус\n4. Вес\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "название сувенира")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новое название сувенира",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о сувенире №{number}:" +
                    "\n1. Название сувенира\n2. Продукт\n3. Вкус\n4. Вес\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "продукт")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый продукт",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о сувенире №{number}:" +
                    "\n1. Название сувенира\n2. Продукт\n3. Вкус\n4. Вес\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вкус")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый вкус",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о сувенире №{number}:" +
                    "\n1. Название сувенира\n2. Продукт\n3. Вкус\n4. Вес\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вес")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый Вес",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о сувенире №{number}:" +
                    "\n1. Название сувенира\n2. Продукт\n3. Вкус\n4. Вес\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSouvenirKeyboard(),
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
                    "Выберите номер сувенира, который хотите изменить",
                    replyMarkup: _keyboardService.GetSouvenirKeyboard(),
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
                    "Выберите номер сувенира, который хотите изменить",
                    replyMarkup: _keyboardService.GetSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о сувенире №{number}:" +
                    "\n1. Название сувенира\n2. Продукт\n3. Вкус\n4. Вес\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditSouvenirKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementSouvenir");
                    await ManagementSouvenirAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                return true;
            }
            if (messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "editSouvenir");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер сувенира, который хотите изменить",
                replyMarkup: _keyboardService.GetSouvenirKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            return false;
        }
    }
}