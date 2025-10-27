using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_bot.Services
{
    public class ManagementHotelService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public ManagementHotelService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task ManagementHotelAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Вот доступные гостиницы:\n\nВыберите, что вы хотите сделать",
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
                    currentSection == "deleteHotel" ||
                    currentSection == "addHotel" ||
                    currentSection == "editHotel")
            {
                if (messageText == "Управление гостиницами")
                {
                    _stateService.SetUserSection(chatId, "managementHotel");
                    await ManagementHotelAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (currentSection == "deleteHotel" || messageText == "Удалить")
                {
                    return await HandleDeleteHotel(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "addHotel" || messageText == "Добавить")
                {
                    return await HandleAddHotel(botClient, chatId, messageText, cancellationToken);
                }

                if (currentSection == "editHotel" || messageText == "Изменить")
                {
                    return await HandleEditHotel(botClient, chatId, messageText, cancellationToken);
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
        private async Task<bool> HandleDeleteHotel(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Удалить")
            {
                _stateService.SetUserSection(chatId, "deleteHotel");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер гостиницы, которую хотите удалить",
                replyMarkup: _keyboardService.GetHotelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }

            if (_stateService.GetUserSection(chatId) == "deleteHotel")
            {
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 4)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Вы уверены, что хотите удалить гостиницу №{number}?",
                    replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Гостиница удалено",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер гостиницы, которую хотите удалить",
                    replyMarkup: _keyboardService.GetHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер гостиницы, которую хотите удалить",
                    replyMarkup: _keyboardService.GetHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementHotel");
                    await ManagementHotelAsync(botClient, chatId, cancellationToken);
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> HandleAddHotel(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (messageText == "Добавить")
            {
                _stateService.SetUserSection(chatId, "addHotel");
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите url гостиницы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите название",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите контактный номер",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите улицу гостиницы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Введите дом гостиницы",
                cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(
                chatId,
                "Сохранить гостиницу?",
                replyMarkup: _keyboardService.GetSaveOrCancelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "сохранить")
            {
                await botClient.SendTextMessageAsync(
                chatId,
                $"Гостиница добавлена",
                cancellationToken: cancellationToken);
                _stateService.SetUserSection(chatId, "managementHotel");
                await ManagementHotelAsync(botClient, chatId, cancellationToken);
                return true;
            }
            if (messageText.ToLower() == "отменить")
            {
                _stateService.SetUserSection(chatId, "managementHotel");
                await ManagementHotelAsync(botClient, chatId, cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<bool> HandleEditHotel(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            if (_stateService.GetUserSection(chatId) == "editHotel")
            {


                if (messageText == "Изменить")
                {
                    _stateService.SetUserSection(chatId, "editHotel");
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите номер гостиницы, которую хотите изменить",
                    replyMarkup: _keyboardService.GetHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int number) && number >= 1 && number <= 12)
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "url гостиницы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый url гостиницы",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
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
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
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
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "улица гостиницы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новую улицу гостиницы",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дом гостиницы")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    $"Введите новый дом гостиницы",
                    cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
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
                    "Выберите номер гостиницы, которую хотите изменить",
                    replyMarkup: _keyboardService.GetHotelKeyboard(),
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
                    "Выберите номер гостиницы, которую хотите изменить",
                    replyMarkup: _keyboardService.GetHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "нет")
                {
                    await botClient.SendTextMessageAsync(
                    chatId,
                    ($"Введите, что вы хотите изменить в информации о гостинице №{number}:" +
                    "\n1. url гостиницы\n2. Название\n3. Контактный номер\n4. Улица гостиницы\n5. Дом гостиницы\nСохранить\nВернуться"),
                    replyMarkup: _keyboardService.GetEditHotelKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    _stateService.SetUserSection(chatId, "managementHotel");
                    await ManagementHotelAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                return true;
            }
            if (messageText == "Изменить")
            {
                _stateService.SetUserSection(chatId, "editHotel");
                await botClient.SendTextMessageAsync(
                chatId,
                "Выберите номер гостиницы, которую хотите изменить",
                replyMarkup: _keyboardService.GetHotelKeyboard(),
                cancellationToken: cancellationToken);
                return true;
            }
            return false;
        }
    }
}