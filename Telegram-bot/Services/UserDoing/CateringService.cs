using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;

namespace Telegram_bot.Services
{
    public class CateringService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public VisitCenterContext context { get; set; }
        public List<Catering> cateringList { get; set; }
        public List<GeneralService.ListId> Caterings { get; set; }

        public CateringService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            context = new VisitCenterContext();
            cateringList = context.Caterings.OrderBy(c => c.IdCatering).ToList();
            Caterings = new List<GeneralService.ListId>();
            int index = 0;
            foreach (var catering in cateringList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = catering.IdCatering;
                listId.IdList = index;
                Caterings.Add(listId);
            }
        }

        public async Task CateringAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            string caterings = "";
            int index = 0;
            foreach(var catering in cateringList)
            {
                index++;
                caterings += $"    {index}. ";
                if (!catering.EstablishmentName.ToLower().Contains(catering.EstablishmentCategory.ToLower()))
                    caterings += $"<b>{catering.EstablishmentCategory} <i>{catering.EstablishmentName}</i></b>";
                else caterings += catering.EstablishmentName;
                caterings += $" (г. Вельск, ул. {catering.EstablishmentStreet}";
                if (!string.IsNullOrEmpty(catering.EstablishmentHouse))
                    caterings += $", д. {catering.EstablishmentHouse})";
                else caterings += ")";
                caterings += "\n";
            }
            await botClient.SendTextMessageAsync(
                chatId,
                $"Вот доступные места общепита:\n\n{caterings}\n\nЕсли вы хотите посмотреть подробную информацию об общепите, выберите его номер:",
                parseMode: ParseMode.Html,
                replyMarkup: _keyboardService.GetKeyboard(cateringList.Count),
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

            if (currentSection == "catering" || messageText.ToLower() == "места общепита")
            {
                if (messageText.ToLower() == "места общепита")
                {
                    _stateService.SetUserSection(chatId, "catering");

                    await CateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (messageText == "Особые дни")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Особые дни общепита:\n\n(Здесь будет список особых дней)",
                        replyMarkup: _keyboardService.GetToCateringKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (messageText == "К местам общепита")
                {
                    await CateringAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (int.TryParse(messageText, out int number))
                {
                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Caterings)
                    {
                        if (number == item.IdList) listId = item;
                    }
                    Catering catering = context.Caterings.Find(listId.IdDB);
                    if ( catering == null )
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b><i>Нет такого заведения! Выберите номер из списка</i></b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetToCateringKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        string cateringDescription = "";
                        if (!catering.EstablishmentName.ToLower().Contains(catering.EstablishmentCategory.ToLower()))
                            cateringDescription += $"<b>{catering.EstablishmentCategory} <i>{catering.EstablishmentName}</i></b>";
                        else cateringDescription += $"<b>{catering.EstablishmentName}</b>";
                        cateringDescription += $"\n\n<b>Виды кухни:</b>";
                        var kitchens = context.CateringTypeKitchens.Include(c => c.IdTypeKitchenNavigation)
                            .Where(c => c.IdCatering == catering.IdCatering)
                            .ToList();
                        foreach ( var kitchen in kitchens )
                            cateringDescription += $"\n    <i>{kitchen.IdTypeKitchenNavigation.NameTypeKitchen}</i>";
                        cateringDescription += $"\n\n<b>Адрес:</b> г. Вельск, ул. {catering.EstablishmentStreet}";
                        if (!string.IsNullOrEmpty(catering.EstablishmentHouse))
                            cateringDescription += $", д. {catering.EstablishmentHouse}";
                        cateringDescription += $"\n\n<b>График работы:</b>";
                        var modeOperations = context.CateringModeOperationCaterings.Include(c => c.IdModeOperationCateringNavigation)
                            .Include(c => c.IdSpecialDayCateringNavigation)
                            .Where(c => c.IdCatering == catering.IdCatering)
                            .OrderBy(c => c.DayWeek)
                            .ToList();
                        if( modeOperations != null )
                        {
                            var specialDays = "";
                            if ((modeOperations.Last().DayWeek == null || modeOperations.Last().DayWeek > 7 || modeOperations.Last().DayWeek <= 0) && modeOperations.Last().IdSpecialDayCatering == null)
                                cateringDescription += $" <i>{modeOperations.Last().IdModeOperationCateringNavigation.Beginning.ToShortTimeString()} " +
                                    $"- {modeOperations.Last().IdModeOperationCateringNavigation.EndDay.ToShortTimeString()}</i>";                            
                            else
                            {
                                //группировка дней недели с одинаковым графиком работы
                                var modes = new List<List<CateringModeOperationCatering>>();
                                List<CateringModeOperationCatering> help = new List<CateringModeOperationCatering>();
                                foreach (var modeOperation in modeOperations)
                                {
                                    if (modeOperation.DayWeek != null && modeOperation.IdSpecialDayCatering == null)
                                    {
                                        if (help.Count == 0)
                                            help.Add(modeOperation);
                                        else if (modeOperation.DayWeek == help.Last().DayWeek + 1 && modeOperation.IdModeOperationCatering == help.First().IdModeOperationCatering)
                                        {
                                            help.Add(modeOperation);
                                        }
                                        else
                                        {
                                            if (help.Count != 0)
                                                modes.Add(new List<CateringModeOperationCatering>(help));
                                            help.Clear();
                                            help.Add(modeOperation);
                                        }
                                    }
                                }
                                
                                if (help.Count != 0)
                                    modes.Add(new List<CateringModeOperationCatering>(help));
                                foreach (var modeOperation in modes)
                                {
                                    int count = modeOperation.Count;
                                    string[] week = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"};
                                    var first = modeOperation.First();
                                    if (count == 1)
                                        cateringDescription += $"\n    <b>{week[(int)first.DayWeek - 1]}:</b> " +
                                            $"<i>{first.IdModeOperationCateringNavigation.Beginning.ToShortTimeString()}" +
                                            $" - {first.IdModeOperationCateringNavigation.EndDay.ToShortTimeString()}</i>";
                                    else if(count > 1)
                                    {
                                        var last = modeOperation.Last();
                                        cateringDescription += $"\n    <b>{week[(int)first.DayWeek - 1]}-{week[(int)last.DayWeek-1]}:</b> " +
                                            $"<i>{first.IdModeOperationCateringNavigation.Beginning.ToShortTimeString()}" +
                                            $" - {first.IdModeOperationCateringNavigation.EndDay.ToShortTimeString()}</i>";
                                    }
                                }
                                
                            }
                            modeOperations = modeOperations
                                    .Where(s => s.IdSpecialDayCatering != null
                                    && s.IdSpecialDayCateringNavigation.Date >= DateOnly.FromDateTime(DateTime.Now)
                                    && s.IdSpecialDayCateringNavigation.Date <= s.IdSpecialDayCateringNavigation.Date.AddDays(14))
                                    .ToList();
                            Console.WriteLine(modeOperations.Count.ToString());
                            if (modeOperations.Count != 0)
                            {
                                foreach (var modeOperation in modeOperations)
                                {
                                    specialDays += $"\n    <b>{modeOperation.IdSpecialDayCateringNavigation.Date}:</b> ";
                                    if (modeOperation.IdModeOperationCatering == null)
                                    {
                                        specialDays += $"Закрыто";
                                    }
                                    else
                                    {
                                        specialDays += $"<i>{modeOperation.IdModeOperationCateringNavigation.Beginning.ToShortTimeString()}" +
                                            $" - {modeOperation.IdModeOperationCateringNavigation.EndDay.ToShortTimeString()}</i>";
                                    }
                                }
                                cateringDescription += specialDays;
                            }

                        }
                        if (!string.IsNullOrEmpty(catering.EstablishmentPhone))
                            cateringDescription += $"\n\n<b>Номер для связи:</b> {catering.EstablishmentPhone}";
                        if (!string.IsNullOrEmpty(catering.CateringUrl))
                            cateringDescription += $"\n\n<b>Более подробная информация:</b> <a href=\"{catering.CateringUrl}\">VK</a>";
                        await botClient.SendTextMessageAsync(
                            chatId,
                            cateringDescription,
                            parseMode: ParseMode.Html,
                            replyMarkup: _keyboardService.GetToCateringKeyboard(),
                            cancellationToken: cancellationToken);
                        return true;
                    }
                        
                }
                else if (messageText.ToLower() == "назад")
                {
                    GeneralService.MainMenuShow(botClient, chatId, cancellationToken, _keyboardService);
                    return true;
                }
            }
            return false;
        }
    }
}