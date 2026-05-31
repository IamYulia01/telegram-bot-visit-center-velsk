using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;

namespace Telegram_bot.Services
{
    public class SouvenirService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public VisitCenterContext context { get; set; }
        public List<Souvenir> souvenirList { get; set; }
        public SouvenirService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            context = new VisitCenterContext();
            souvenirList = context.Souvenirs.ToList();
        }

        public async Task SouvenirAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            string souvenirs = "";
            int i = 0;
            if (souvenirList == null || !souvenirList.Any())
            {
                souvenirs = "На данный момент у нас нет сувениров в каталоге!";
            }
            else
            {
                foreach (var souvenir in souvenirList)
                {
                    i++;
                    souvenirs += $"    {i}. ";
                    if (!souvenir.NameSouvenir.ToLower().Contains(souvenir.Product.ToLower()))
                        souvenirs += $"<b>{souvenir.Product} <i>{souvenir.NameSouvenir}</i></b>";
                    else souvenirs += $"<b>{souvenir.NameSouvenir}</b>\n";
                    if (!string.IsNullOrEmpty(souvenir.Tastes))
                        souvenirs += $"\n<b>Вкус:</b> <i>{souvenir.Tastes}</i>";
                    if (!string.IsNullOrEmpty(souvenir.Weight))
                        souvenirs += $"\n<b>Вес:</b> <i>{souvenir.Weight}</i>";
                    souvenirs += $"\n\n";
                }
            }
                
            await botClient.SendTextMessageAsync(
                chatId,
                $"Вот доступные сувениры: \n\n{souvenirs}",
                parseMode: ParseMode.Html,
                replyMarkup: _keyboardService.GetBackKeyboard(),
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

            if (currentSection == "souvenir" || messageText.ToLower() == "сувениры")
            {
                if (messageText.ToLower() == "сувениры")
                {
                    _stateService.SetUserSection(chatId, "souvenir");
                    await SouvenirAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад")
                {
                    GeneralService.MainMenuShow(botClient, chatId, cancellationToken, _keyboardService);
                    return true;
                }
            }
            return false;
        }
    }
}