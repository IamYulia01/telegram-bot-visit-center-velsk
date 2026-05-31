using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;

namespace Telegram_bot.Services
{
    public class HotelService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public VisitCenterContext context { get; set; }
        public GeneralService generalService { get; set; }
        public List<Hotel> hotelList { get; set; }
        public List<GeneralService.ListId> Hotels { get; set; }
        private readonly Dictionary<int, List<string>> photoCache = new();
        public HotelService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            generalService = new GeneralService();
            context = new VisitCenterContext();
            hotelList = context.Hotels.ToList();
            Hotels = new List<GeneralService.ListId>();
            int index = 0;
            foreach (var hotel in hotelList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = hotel.IdHotel;
                listId.IdList = index;
                Hotels.Add(listId);
            }
            PhotoCacheHotel();
        }
        private void PhotoCacheHotel()
        {
            var allPhotos = context.PhotoHotels.ToList();
            foreach (var photo in allPhotos)
            {
                if (!photoCache.ContainsKey(photo.IdHotel))
                    photoCache[photo.IdHotel] = new List<string>();
                photoCache[photo.IdHotel].Add(photo.NameFile);
            }
        }
        public async Task HotelAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            string hotels = "";
            int index = 0;
            foreach (var hotel in hotelList)
            {
                index++;
                hotels += $"    {index}. ";
                if (!hotel.HotelName.ToLower().Contains(hotel.TypeHotel.ToLower()))
                    hotels += $"<b>{hotel.TypeHotel} <i>{hotel.HotelName}</i></b>";
                else hotels += hotel.HotelName;
                hotels += $" (г. Вельск, ул. {hotel.HotelStreet}";
                if (!string.IsNullOrEmpty(hotel.HotelHouse))
                    hotels += $", д. {hotel.HotelHouse})";
                else hotels += ")";
                hotels += "\n";
            }
            await botClient.SendTextMessageAsync(
                chatId,
                $"Вот доступные гостиницы:\n\n{hotels}\n\nЕсли вы хотите посмотреть подробную информацию о гостинице, выберите её номер:",
                parseMode: ParseMode.Html,
                replyMarkup: _keyboardService.GetKeyboard(hotelList.Count),
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

            if (currentSection == "hotel" || messageText.ToLower() == "гостиницы")
            {
                if (messageText.ToLower() == "гостиницы")
                {
                    _stateService.SetUserSection(chatId, "hotel");
                    await HotelAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (messageText == "К гостиницам")
                {
                    await HotelAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                else if (int.TryParse(messageText, out int number))
                {
                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Hotels)
                    {
                        if (number == item.IdList) listId = item;
                    }
                    Hotel hotel = context.Hotels.Find(listId.IdDB);
                    if (hotel == null)
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b><i>Нет такого заведения! Выберите номер из списка</i></b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetToHotelKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        string hotelDescription = "";
                        if (!hotel.HotelName.ToLower().Contains(hotel.TypeHotel.ToLower()))
                            hotelDescription += $"<b>{hotel.TypeHotel} <i>{hotel.HotelName}</i></b>";
                        else hotelDescription += $"<b>{hotel.HotelName}</b>";
                        hotelDescription += $"\n\n<b>Адрес:</b> г. Вельск, ул. {hotel.HotelStreet}";
                        if (!string.IsNullOrEmpty(hotel.HotelHouse))
                            hotelDescription += $", д. {hotel.HotelHouse}";
                        if (!string.IsNullOrEmpty(hotel.ContactNumberHotel))
                            hotelDescription += $"\n\n<b>Номер для связи:</b> {hotel.ContactNumberHotel}";
                        if (!string.IsNullOrEmpty(hotel.HotelUrl))
                            hotelDescription += $"\n\n<b>Более подробная информация:</b> <a href=\"{hotel.HotelUrl}\">VK</a>";
                        if(!photoCache.TryGetValue(hotel.IdHotel, out var photoLinks))
                        {
                            photoLinks = context.PhotoHotels
                                .Where(p=>p.IdHotel == hotel.IdHotel)
                                .Select(p=>p.NameFile)
                                .ToList();
                            photoCache[hotel.IdHotel] = photoLinks;
                        }
                        if (photoLinks.Any())
                        {
                            Console.WriteLine("Отправляем фото");
                            await generalService.SendPhoto(
                                photoLinks,
                                botClient,
                                chatId,
                                hotel.HotelName,
                                cancellationToken
                                );
                        }
                        else
                            Console.WriteLine("Нет фото для отправки");
                        Console.WriteLine("Отправляем текстовое описание");
                        await botClient.SendTextMessageAsync(
                            chatId,
                            hotelDescription,
                            parseMode: ParseMode.Html,
                            replyMarkup: _keyboardService.GetToHotelKeyboard(),
                            cancellationToken: cancellationToken);
                        Console.WriteLine("Обработка завершена");
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