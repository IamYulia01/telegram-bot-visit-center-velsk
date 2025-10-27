using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Services;

class Program
{
    private static KeyboardService _keyboardService = new KeyboardService();
    private static StateService _stateService = new StateService();
    private static TelegramBotClient botClient = new TelegramBotClient("8094977616:AAH8oU0SKpqCP299sYtZIKCeklL3IQx-mMM");
    private static readonly string botToken = "8094977616:AAH8oU0SKpqCP299sYtZIKCeklL3IQx-mMM";
    static async Task Main(string[] args)
    {
        try
        {
            botClient = new TelegramBotClient(botToken);
            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Бот @{me.Username} запущен!");
            Console.WriteLine("Нажмите Enter для остановки...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА: {ex.Message}");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        var currentSection = _stateService.GetUserSection(chatId);

        Console.WriteLine($"Получено сообщение '{messageText}' в чате {chatId}");

        if (update.Type == UpdateType.Message)
        {
            if (messageText.ToLower() == "/start")
            {
                _stateService.ResetUserSection(chatId);
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Добро пожаловать в визит-центр!",
                    cancellationToken: cancellationToken);
                await MainMenuAsync(botClient, chatId, cancellationToken);
                return;
            }
            if (messageText == "/razrab")
            {
                _stateService.SetUserSection(chatId, "management");
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Добро пожаловать в режим разработчика!",
                    cancellationToken: cancellationToken);
                await DeveloperMenuAsync(botClient, chatId, cancellationToken);
                return;
            }

            if (messageText.ToLower() == "выйти" && currentSection.StartsWith("management"))
            {
                _stateService.ResetUserSection(chatId);
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Вы вышли из режима разработчика",
                    cancellationToken: cancellationToken);
                await MainMenuAsync(botClient, chatId, cancellationToken);
                return;
            }

            if (currentSection.StartsWith("management") || messageText == "Управление достопримечательностями")
            {
                Console.WriteLine($"Режим разработчика. Обработка Management сервисами...");
                if (await TryHandleInService<ManagementSightService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementEventService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementHotelService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementCateringService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementSouvenirService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementModeOperationService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementSpecialDayService>(botClient, update, cancellationToken)) return;
                if (await TryHandleInService<ManagementTicketService>(botClient, update, cancellationToken)) return;

                await DeveloperMenuAsync(botClient, chatId, cancellationToken);
                return;
            }
            if (await TryHandleInService<SightService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<EventService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<HotelService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<CateringService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<SouvenirService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<UserProfileService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<RouteService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<FeedbackService>(botClient, update, cancellationToken)) return;

            await MainMenuAsync(botClient, chatId, cancellationToken);

        }
    }
    private static async Task<bool> TryHandleInService<T>(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var service = Activator.CreateInstance(typeof(T), _keyboardService, _stateService) as dynamic;
            if (service != null)
            {
                bool handled = await service.TryHandleMessageAsync(botClient, update, cancellationToken);
                if (handled)
                {
                    Console.WriteLine($"Обработано в {typeof(T).Name}");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в {typeof(T).Name}: {ex.Message}");
        }
        return false;
    }

    private static async Task MainMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {

        await botClient.SendTextMessageAsync(
            chatId,
            "Выберите действие:\n1. Достопримечательности\n2. Мероприятия\n3. Гостиницы\n4. Места общепита\n5. Сувениры\n6. Анкета\n7. Индивидуальные маршруты\n8. Обратная связь",
            replyMarkup: _keyboardService.GetMainMenuKeyboard(),
            cancellationToken: cancellationToken);
    }
    private static async Task DeveloperMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {

        await botClient.SendTextMessageAsync(
            chatId,
            "Выберите действие:\n1. Управление достопримечательностями\n2. Управление мероприятиями\n3. Управление гостиницами\n4. Управление местами общепита\n5. Управление сувенирами\n6. Управление особыми днями\n7. Управление рабочими графиками\n8. Управление билетами\n9. Выйти",
            replyMarkup: _keyboardService.GetMainMenuDeveloperKeyboard(),
            cancellationToken: cancellationToken);
    }
    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
