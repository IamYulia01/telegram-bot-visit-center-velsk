using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot;
using Telegram_bot.Models;
using Telegram_bot.Services;

class Program
{

    private static KeyboardService _keyboardService = new KeyboardService();
    private static StateService _stateService = new StateService();
    private static GeneralService _generalService = new GeneralService();

    private static Userbot user = new Userbot();
    public static VisitCenterContext context = new VisitCenterContext();
    private static TelegramBotClient botClient = new TelegramBotClient("8094977616:AAH8oU0SKpqCP299sYtZIKCeklL3IQx-mMM");
    private static readonly string botToken = "8094977616:AAH8oU0SKpqCP299sYtZIKCeklL3IQx-mMM";
    static async Task Main(string[] args)
    {
        try
        {
            
            botClient = new TelegramBotClient(botToken);

            Console.WriteLine($"Путь к медиафайлам: {_generalService.mediaPath}");
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
        user = context.Userbots.Find(chatId);
        if(user == null)
        {
            user = new Userbot();
            user.IdUser = chatId;
            context.Userbots.Add(user);
            context.SaveChanges();
        }

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
            if (await TryHandleInService<SightService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<EventService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<HotelService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<CateringService>(botClient, update, cancellationToken)) return;
            if (await TryHandleInService<SouvenirService>(botClient, update, cancellationToken)) return;
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
            GeneralService.MainMenu,
            parseMode: ParseMode.Html,
            replyMarkup: _keyboardService.GetMainMenuKeyboard(),
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
