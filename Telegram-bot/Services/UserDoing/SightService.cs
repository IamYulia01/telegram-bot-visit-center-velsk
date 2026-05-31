using Microsoft.EntityFrameworkCore;
using System.Security;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;

namespace Telegram_bot.Services
{
    public class SightService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public VisitCenterContext context { get; set; }
        public GeneralService generalService { get; set; }
        public List<Sight> sightList { get; set; }
        public string mediaPath { get; set; }
        private readonly Dictionary<int, List<string>> photoCache = new();
        
        public List<GeneralService.ListId> Sights { get; set; }

        public SightService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            generalService = new GeneralService();
            context = new VisitCenterContext();
            sightList = context.Sights.OrderBy(s => s.IdSight).ToList();
            mediaPath = generalService.FindMediaFolder();
            Sights = new List<GeneralService.ListId>();
            int index = 0;
            foreach (var sight in sightList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = sight.IdSight;
                listId.IdList = index;
                Sights.Add(listId);
            }
            PhotoCache();
        }
        public string GetMediaPath()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;
            string mediaFolder = Path.Combine(projectRoot, "..", "system_management_information", "system_management_information", "Media");
            mediaFolder = Path.GetFullPath(mediaFolder);
            return mediaFolder;
        }

        private async Task SightAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            string sights = "";
            int index = 0;
            var sightsList = new List<GeneralService.ListId>();
            foreach (var sight in sightList)
            {
                index++;
                sights += $"    {index}. ";
                if (!sight.NameSight.ToLower().Contains(sight.TypeSight.ToLower()))
                    sights += $"<b>{sight.TypeSight} <i>{sight.NameSight}</i></b>";
                else sights += $"<b><i>{sight.NameSight}</i></b>";
                sights += $" (г. Вельск, ул. {sight.LocationStreet}";
                if (!string.IsNullOrEmpty(sight.LocationHouse))
                    sights += $", д. {sight.LocationHouse})";
                else sights += ")";
                sights += "\n";
                
            }
            Sights = sightsList;
            await botClient.SendTextMessageAsync(
                chatId,
                $"Вот доступные достопримечательности:\n\n{sights}\n\nЕсли вы хотите посмотреть подробную информацию о достопримечательности, выберите её номер:",
                parseMode: ParseMode.Html,
                replyMarkup: _keyboardService.GetKeyboard(sightList.Count),
                cancellationToken: cancellationToken);
        }
        private void PhotoCache()
        {
            var allPhotos = context.PhotoSights.ToList();
            foreach (var photo in allPhotos)
            {
                if (!photoCache.ContainsKey(photo.IdSight))
                    photoCache[photo.IdSight] = new List<string>();
                photoCache[photo.IdSight].Add(photo.LinkPhoto);
            }
        }
        public async Task<bool> TryHandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return false;

            if (message.Text is not { } messageText)
                return false;

            var chatId = message.Chat.Id;
            var currentSection = _stateService.GetUserSection(chatId);

            if (currentSection == "sight" || messageText.ToLower() == "достопримечательности")
            {
                if (messageText.ToLower() == "достопримечательности")
                {
                    _stateService.SetUserSection(chatId, "sight");
                    await SightAsync(botClient, chatId, cancellationToken);
                    return true;
                }

                if (messageText == "Особые дни")
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Особые дни достопримечательности:\n\n(Здесь будет список особых дней)",
                        replyMarkup: _keyboardService.GetToSightKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (messageText == "К достопримечательностям")
                {
                    await SightAsync(botClient, chatId, cancellationToken);

                    return true;
                }
                if (int.TryParse(messageText, out int number))
                {

                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Sights)
                    {
                        if (number == item.IdList) listId = item;
                    }
                    Sight sight = context.Sights.Find(listId.IdDB);                    
                    if (sight == null)
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b><i>Нет такой достопримечательности! Выберите номер из списка</i></b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetToSightKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        string sightDescription = "";
                        if (!sight.NameSight.ToLower().Contains(sight.TypeSight.ToLower()))
                            sightDescription += $"<b>{sight.TypeSight} <i>{sight.NameSight}</i></b>";
                        else sightDescription += $"<b><i>{sight.NameSight}</i></b>";
                        if (!string.IsNullOrEmpty(sight.Description))
                            sightDescription += $"\n\n{sight.Description}";
                        sightDescription += $"\n\n<b>Адрес:</b> г. Вельск, ул. {sight.LocationStreet}";
                        if (!string.IsNullOrEmpty(sight.LocationHouse))
                            sightDescription += $", д. {sight.LocationHouse}";
                        sightDescription += $"\n\n<b>График работы:</b>";
                        var modeOperations = context.SightOperatingModes.Include(s => s.IdOperatingModeNavigation)
                            .Include(s => s.IdSpecialDaySightNavigation)
                            .Where(s => s.IdSight == sight.IdSight)
                            .OrderBy(s => s.WorkingDayWeek)
                            .ToList();
                        
                        if (modeOperations != null && modeOperations.Count != 0)
                        {
                            string specialDays = "";
                            if ((modeOperations.Last().WorkingDayWeek == null || modeOperations.Last().WorkingDayWeek > 7 || modeOperations.Last().WorkingDayWeek <= 0) && modeOperations.Last().IdSpecialDaySight == null)
                                sightDescription += $" {modeOperations.Last().IdOperatingModeNavigation.StartTime.ToShortTimeString()} " +
                                    $"- {modeOperations.Last().IdOperatingModeNavigation.EndTime.ToShortTimeString()}";
                            else
                            {
                                //группировка дней недели с одинаковым графиком работы
                                var modes = new List<List<SightOperatingMode>>();
                                List<SightOperatingMode> help = new List<SightOperatingMode>();
                                foreach (var modeOperation in modeOperations)
                                {
                                    if (modeOperation.WorkingDayWeek != null && modeOperations.Last().IdSpecialDaySight == null)
                                    {
                                        if (help.Count == 0)
                                            help.Add(modeOperation);
                                        else if (modeOperation.WorkingDayWeek == help.Last().WorkingDayWeek + 1 && modeOperation.IdOperatingMode == help.First().IdOperatingMode)
                                        {
                                            help.Add(modeOperation);
                                        }
                                        else
                                        {
                                            if (help.Count != 0)
                                                modes.Add(new List<SightOperatingMode>(help));
                                            help.Clear();
                                            help.Add(modeOperation);
                                        }
                                    }
                                }
                                if (help.Count != 0)
                                    modes.Add(new List<SightOperatingMode>(help));
                                foreach (var modeOperation in modes)
                                {
                                    int count = modeOperation.Count;
                                    string[] week = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
                                    var first = modeOperation.First();
                                    if (count == 1)
                                        sightDescription += $"\n    <b>{week[(int)first.WorkingDayWeek - 1]}:</b> " +
                                            $"{first.IdOperatingModeNavigation.StartTime.ToShortTimeString()}" +
                                            $" - {first.IdOperatingModeNavigation.EndTime.ToShortTimeString()}";
                                    else if (count > 1)
                                    {
                                        var last = modeOperation.Last();
                                        sightDescription += $"\n    <b>{week[(int)first.WorkingDayWeek - 1]}-{week[(int)last.WorkingDayWeek - 1]}:</b> " +
                                            $"<i>{first.IdOperatingModeNavigation.StartTime.ToShortTimeString()}" +
                                            $" - {first.IdOperatingModeNavigation.EndTime.ToShortTimeString()}</i>";
                                    }
                                }
                            }
                            modeOperations = modeOperations
                                    .Where(s => s.IdSpecialDaySight != null
                                    && s.IdSpecialDaySightNavigation.SpecialDayDate >= DateOnly.FromDateTime(DateTime.Now)
                                    && s.IdSpecialDaySightNavigation.SpecialDayDate <= s.IdSpecialDaySightNavigation.SpecialDayDate.AddDays(14))
                                    .ToList();
                            Console.WriteLine(modeOperations.Count.ToString());
                            if (modeOperations.Count != 0)
                            {
                                foreach (var modeOperation in modeOperations)
                                {
                                    specialDays += $"\n    <b>{modeOperation.IdSpecialDaySightNavigation.SpecialDayDate}:</b> ";
                                    if (modeOperation.IdOperatingMode == null)
                                    {
                                        specialDays += $"Закрыто";
                                    }
                                    else
                                    {
                                        specialDays += $"<i>{modeOperation.IdOperatingModeNavigation.StartTime.ToShortTimeString()}" +
                                            $" - {modeOperation.IdOperatingModeNavigation.EndTime.ToShortTimeString()}</i>";
                                    }
                                }
                                sightDescription += specialDays;
                            }
                        }
                        else sightDescription += $" Не указан";                        
                        if (!string.IsNullOrEmpty(sight.ContactNumber))
                            sightDescription += $"\n\n<b>Номер для связи:</b> {sight.ContactNumber}";
                        if (!string.IsNullOrEmpty(sight.Email))
                            sightDescription += $"\n\n<b>e-mail:</b> {sight.Email}";
                        if (!string.IsNullOrEmpty(sight.SightUrl))
                            sightDescription += $"\n\n<b>Более подробная информация:</b> <a href=\"{sight.SightUrl}\">VK</a>";
                        if(!photoCache.TryGetValue(sight.IdSight, out var photoLinks))
                        {
                            photoLinks = context.PhotoSights
                                .Where(p => p.IdSight == sight.IdSight)
                                .Select(p => p.LinkPhoto)
                                .ToList();
                            photoCache[sight.IdSight] = photoLinks;
                        }
                        if (photoLinks.Any())
                        {
                            Console.WriteLine("Отправляем фото");
                            await generalService.SendPhoto(
                                photoLinks,
                                botClient,
                                chatId,
                                sight.NameSight,
                                cancellationToken
                                );
                        }
                        else
                            Console.WriteLine("Нет фото для отправки");
                        await botClient.SendTextMessageAsync(
                        chatId,
                        sightDescription,
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetToSightKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                        
                    }
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