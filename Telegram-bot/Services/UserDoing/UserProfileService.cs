using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Services;

namespace Telegram_bot.Services
{
    public class UserProfileService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;

        public UserProfileService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
        }

        private async Task UserProfileAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Ваша анкета: \n\n (Здесь будет анкета, если она заполнена)",
                replyMarkup: _keyboardService.GetUserProfileKeyboard(),
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
            if (currentSection == "userProfile" || messageText == "Анкета")
            {
                if (messageText == "Анкета")
                {
                    _stateService.SetUserSection(chatId, "userProfile");
                    await UserProfileAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "создать анкету")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Вы уверены, что хотите заполнить анкету? Если анкета была заполнена ранее, то данные будут удалены.",
                        replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "да")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите фамилию",
                        replyMarkup: _keyboardService.GetSkipKeyboard(),
                        cancellationToken: cancellationToken);

                    if (update.Type == UpdateType.Message)
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите имя",
                        replyMarkup: _keyboardService.GetSkipKeyboard(),
                        cancellationToken: cancellationToken);
                    }
                    if (update.Type == UpdateType.Message)
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите отчество",
                        replyMarkup: _keyboardService.GetSkipKeyboard(),
                        cancellationToken: cancellationToken);
                    }
                    if (update.Type == UpdateType.Message)
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите номер телефона",
                        replyMarkup: _keyboardService.GetSkipKeyboard(),
                        cancellationToken: cancellationToken);
                    }
                    if (update.Type == UpdateType.Message)
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите дату рождения",
                        replyMarkup: _keyboardService.GetSkipKeyboard(),
                       cancellationToken: cancellationToken);
                    }
                }
                if (messageText.ToLower() == "нет")
                {
                    await UserProfileAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "изменить анкету")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                        replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Анкета не была сохранена",
                        cancellationToken: cancellationToken);
                    await UserProfileAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "сохранить")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Анкета была сохранена",
                        cancellationToken: cancellationToken);
                    await UserProfileAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "фамилия")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите фамилию:",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                    replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "имя")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите имя:",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                    replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "отчество")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите отчество:",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                    replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "пол")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите пол:",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                    replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "номер телефона")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите номер телефона:",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                    replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "дата рождения")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите дату рождения:",
                        cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(
                    chatId,
                    "Выберите, что вы хотите изменить: \n1.Фамилия\n2.Имя\n3.Отчество\n4.Номер телефона\n5.Пол\n6.Дата рождения",
                    replyMarkup: _keyboardService.GetEditUserProfileKeyboard(),
                    cancellationToken: cancellationToken);
                    return true;
                }
            }
            return false;
        }
    }
}