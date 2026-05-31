using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;
using Telegram_bot.Services;
using Telegram_bot.Services.UserDoing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Telegram_bot.Services
{
    public class RouteService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        private readonly EmailService _emailService;

        public List<Sight> sightList { get; set; }
        public VisitCenterContext context { get; set; }
        public static string typePlace;
        public static string nameRoute;
        public static string routesInfo;
        public static string namesRoute;

        public Userbot user { get; set; } = new Userbot();

         public Route route { get; set; } = new Route();
        public  static Route routeForDelete { get; set; } = new Route();
        public List<Hotel> hotelList { get; set; }
        public List<Event> eventList { get; set; }
        public List<Catering> cateringList { get; set; }
        public List<Route> routesUser { get; set; }
        public List<GeneralService.ListId> Hotels { get; set; }
        public List<GeneralService.ListId> Sights { get; set; }
        public List<GeneralService.ListId> Caterings { get; set; }
        public List<GeneralService.ListId> Events { get; set; }


        public static List<RouteCateringHotel> routeCateringHotels = new List<RouteCateringHotel>();
        public static List<RouteEventSight> routeEventSight = new List<RouteEventSight>();

        public RouteService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            _emailService = new EmailService();
            context = new VisitCenterContext();
            routesUser = new List<Route>();
            sightList = context.Sights.OrderBy(s => s.IdSight).ToList();
            hotelList = context.Hotels.ToList();
            eventList = context.Events.Where(p => p.DateEvent >= DateOnly.FromDateTime(DateTime.Now) || p.DateEvent == null).OrderBy(c => c.IdEvent).ToList();
            cateringList = context.Caterings.OrderBy(c => c.IdCatering).ToList();

            int index = 0;
            Sights = new List<GeneralService.ListId>();
            foreach (var sight in sightList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = sight.IdSight;
                listId.IdList = index;
                Sights.Add(listId);
            }
            index = 0;
            Hotels = new List<GeneralService.ListId>();
            foreach (var hotel in hotelList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = hotel.IdHotel;
                listId.IdList = index;
                Hotels.Add(listId);
            }
            index = 0;
            Caterings = new List<GeneralService.ListId>();
            foreach (var catering in cateringList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = catering.IdCatering;
                listId.IdList = index;
                Caterings.Add(listId);
            }
            index = 0;
            Events = new List<GeneralService.ListId>();
            foreach (var _event in eventList)
            {
                index++;
                GeneralService.ListId listId = new GeneralService.ListId();
                listId.IdDB = _event.IdEvent;
                listId.IdList = index;
                Events.Add(listId);
            }
        }

        private async Task RouteAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
            routesInfo = "";
            namesRoute = "";
            routesUser = context.Routes.Where(r => r.IdUser == user.IdUser).ToList();
            if(routesUser.Count == 0)
            {
                routesInfo += "\n\n    <b>У вас пока нет индивидуальных маршрутов!</b>";                
            }
            else
            {
                int numberRoute = 0;
                foreach (var route in routesUser)
                {
                    numberRoute++;
                    if (string.IsNullOrEmpty(route.NameRoute))
                    {
                        routesInfo += $"\n\n<b>Маршрут №{numberRoute}</b>";
                        namesRoute += $"\n    <b>Маршрут №{numberRoute}</b>";
                    }
                    else
                    {
                        routesInfo += $"\n\n<b>{route.NameRoute}</b>";
                        namesRoute += $"\n    <b>{route.NameRoute}</b>";
                    }

                    var sights = context.RouteEventSights.Include(r => r.IdSightNavigation).Where(r => r.IdRoute == route.IdRoute && r.IdSight != null);
                    if (sights.Count() != 0)
                    {
                        int numberSight = 0;
                        routesInfo += $"\n<b><i>Достопримечательности:</i></b>";
                        foreach (var sight in sights)
                        {
                            numberSight++;
                            var sightInfo = sight.IdSightNavigation;
                            routesInfo += $"\n    {numberSight}. ";
                            if (!sightInfo.NameSight.ToLower().Contains(sightInfo.TypeSight.ToLower()))
                                routesInfo += $"<b>{sightInfo.TypeSight} <i>{sightInfo.NameSight}</i></b>";
                            else routesInfo += $"<b><i>{sightInfo.NameSight}</i></b>";
                            routesInfo += $" (г. Вельск, ул. {sightInfo.LocationStreet}";
                            if (!string.IsNullOrEmpty(sightInfo.LocationHouse))
                                routesInfo += $", д. {sightInfo.LocationHouse})";
                            else routesInfo += ")";
                        }
                    }

                    var events = context.RouteEventSights.Include(r => r.IdEventNavigation).Where(r => r.IdRoute == route.IdRoute && r.IdEvent != null);
                    if (events.Count() != 0)
                    {
                        int numberEvent = 0;
                        routesInfo += $"\n<b><i>Мероприятия:</i></b>";
                        foreach (var _event in events)
                        {
                            numberEvent++;
                            var eventInfo = _event.IdEventNavigation;
                            routesInfo += $"\n    {numberEvent}. ";
                            if (!eventInfo.NameEvent.ToLower().Contains(eventInfo.TypeEvent.ToLower()))
                                routesInfo += $"<b>{eventInfo.TypeEvent} <i>{eventInfo.NameEvent}</i></b>";
                            else routesInfo += $"<b>{eventInfo.NameEvent}</b>\n";
                            routesInfo += $" (г. Вельск, ул. {eventInfo.StreetEvent}";
                            if (!string.IsNullOrEmpty(eventInfo.HouseEvent))
                                routesInfo += $", д. {eventInfo.HouseEvent})";
                            else routesInfo += ")";
                        }
                    }

                    var caterings = context.RouteCateringHotels.Include(r => r.IdCateringNavigation).Where(r => r.IdRoute == route.IdRoute && r.IdCatering != null);
                    if (caterings.Count() != 0)
                    {
                        int numberCatering = 0;
                        routesInfo += $"\n<b><i>Места общепита:</i></b>";
                        foreach (var catering in caterings)
                        {
                            numberCatering++;
                            var cateringInfo = catering.IdCateringNavigation;
                            routesInfo += $"\n    {numberCatering}. ";
                            if (!cateringInfo.EstablishmentName.ToLower().Contains(cateringInfo.EstablishmentCategory.ToLower()))
                                routesInfo += $"<b>{cateringInfo.EstablishmentCategory} <i>{cateringInfo.EstablishmentName}</i></b>";
                            else routesInfo += cateringInfo.EstablishmentName;
                            routesInfo += $" (г. Вельск, ул. {cateringInfo.EstablishmentStreet}";
                            if (!string.IsNullOrEmpty(cateringInfo.EstablishmentHouse))
                                routesInfo += $", д. {cateringInfo.EstablishmentHouse})";
                            else routesInfo += ")";
                        }
                    }

                    var hotels = context.RouteCateringHotels.Include(r => r.IdHotelNavigation).Where(r => r.IdRoute == route.IdRoute && r.IdHotel != null);
                    if (hotels.Count() != 0)
                    {
                        int numberHotel = 0;
                        routesInfo += $"\n<b><i>Гостиницы:</i></b>";
                        foreach (var hotel in hotels)
                        {
                            numberHotel++;
                            var hotelInfo = hotel.IdHotelNavigation;
                            routesInfo += $"\n    {numberHotel}. ";
                            if (!hotelInfo.HotelName.ToLower().Contains(hotelInfo.TypeHotel.ToLower()))
                                routesInfo += $"<b>{hotelInfo.TypeHotel} <i>{hotelInfo.HotelName}</i></b>";
                            else routesInfo += hotelInfo.HotelName;
                            routesInfo += $" (г. Вельск, ул. {hotelInfo.HotelStreet}";
                            if (!string.IsNullOrEmpty(hotelInfo.HotelHouse))
                                routesInfo += $", д. {hotelInfo.HotelHouse})";
                            else routesInfo += ")";
                        }
                    }
                }
                
            }
            await botClient.SendTextMessageAsync(
                chatId,
                $"Ваши индивидуальные маршруты: {routesInfo}",
                parseMode: ParseMode.Html,
                replyMarkup: _keyboardService.GetRouteKeyboard(),
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
            user = context.Userbots.Find(chatId);
            if (currentSection == "route" || messageText.ToLower() == "индивидуальные маршруты")
            {
                if (messageText.ToLower() == "индивидуальные маршруты")
                {
                    _stateService.SetUserSection(chatId, "route");
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "вернуться")
                {
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "отменить")
                {
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "назад" && (typePlace == "гостиница" || typePlace == "мероприятие" || typePlace == "место общепита" || typePlace == "достопримечательность"))
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМероприятия\nГостиницы\nМеста общепита\nСувениры",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                else if (messageText.ToLower() == "назад")
                {
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "в главное меню")
                {
                    GeneralService.MainMenuShow(botClient, chatId, cancellationToken, _keyboardService);
                    return true;
                }
                if (messageText.ToLower() == "создать маршрут")
                {
                    routeEventSight.Clear();
                    routeCateringHotels.Clear();
                    typePlace = "название маршрута";
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Введите название маршрута",
                        replyMarkup: _keyboardService.GetBackOrSkipKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;                    
                }
                
                if (messageText.ToLower() == "достопримечательность")
                {
                    typePlace = "достопримечательность";
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
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"Вот список доступных достопримечательностей: \n\n{sights}\n\nЕсли хотите добавить достопримечательность в маршрут, выберите её номер",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetKeyboard(sightList.Count),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "мероприятие")
                {
                    typePlace = "мероприятие";
                    string events = "";
                    int i = 0;
                    foreach (var eventFuture in eventList)
                    {                        
                        i++;
                        var tickets = context.Tickets.Where(t => t.IdEvent == eventFuture.IdEvent).ToList();
                        events += $"    {i}. ";
                        if (!eventFuture.NameEvent.ToLower().Contains(eventFuture.TypeEvent.ToLower()))
                            events += $"<b>{eventFuture.TypeEvent} <i>{eventFuture.NameEvent}</i></b>";
                        else events += $"<b>{eventFuture.NameEvent}</b>\n";
                        events += $"\n<b>Адрес проведения:</b> г. Вельск, ул. {eventFuture.StreetEvent}";
                        if (!string.IsNullOrEmpty(eventFuture.HouseEvent))
                            events += $", д. {eventFuture.HouseEvent}";
                        if (eventFuture.DateEvent != null)
                            events += $"\n<b>Дата проведения:</b> {eventFuture.DateEvent}";
                        if (!string.IsNullOrEmpty(eventFuture.AgeLimit))
                            events += $"\n<b>Возрастное ограничение:</b> <i>{eventFuture.AgeLimit}</i>";
                        if (tickets != null && tickets.Any())
                        {
                            events += $"\n<b>Билеты:</b>";
                            foreach (var ticket in tickets)
                            {
                                events += "\n    ";
                                if (ticket.MinimumAge != null && ticket.MinimumAge > 0 && ticket.MaximumAge != null && ticket.MaximumAge < 100)
                                    events += $"<b>От {ticket.MinimumAge} до {ticket.MaximumAge} лет:</b>";
                                else if ((ticket.MinimumAge == null || ticket.MinimumAge <= 0) && ticket.MaximumAge != null && ticket.MaximumAge < 100)
                                    events += $"<b>До {ticket.MaximumAge} лет:</b>";
                                else if (ticket.MinimumAge != null && ticket.MinimumAge > 0 && (ticket.MaximumAge == null || ticket.MaximumAge >= 100))
                                    events += $"<b>От {ticket.MinimumAge} лет:</b>";
                                if (ticket.Price != 0 && ticket.Price != null)
                                {
                                    events += $"  <i>{ticket.Price.Value.ToString("F2")} руб</i>";
                                    if (ticket.CountPeople != 0 && ticket.CountPeople != null)
                                        events += $" за {ticket.CountPeople} чел.";
                                }
                                else events += $"  <i>Бесплатно</i>";

                            }
                        }
                        events += "\n\n";
                    }
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"Вот список доступных мероприятий: \n\n{events}\n\nЕсли хотите добавить мероприятие в маршрут, выберите его номер",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetKeyboard(eventList.Count),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "место общепита")
                {
                    typePlace = "место общепита";
                    string caterings = "";
                    int index = 0;
                    foreach (var catering in cateringList)
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
                        $"Вот спиcок доступных мест общепита: \n\n{caterings}\n\nЕсли хотите добавить общепит в маршрут, выберите его номер",
                        parseMode: ParseMode.Html, 
                        replyMarkup: _keyboardService.GetKeyboard(cateringList.Count),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (messageText.ToLower() == "гостиница")
                {
                    typePlace = "гостиница";
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
                        $"Вот список доступных гостиниц: \n\n{hotels}\n\nЕсли хотите добавить гостиницу в маршрут, выберите её номер",
                        replyMarkup: _keyboardService.GetKeyboard(hotelList.Count),
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (typePlace == "название маршрута")
                {
                    if (messageText.ToLower() != "пропустить" && messageText.ToLower() != "отменить") 
                    {
                        var existingRoutes = context.Routes.Where(r => r.IdUser == user.IdUser).ToList();
                        if (!existingRoutes.Select(r => r.NameRoute).Contains(messageText))
                            nameRoute = messageText;
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId,
                                $"<b>У вас уже есть маршрут с таким названием! Придумайте другое название!</b>",
                                parseMode: ParseMode.Html,
                                replyMarkup: _keyboardService.GetBackOrSkipKeyboard(),
                                cancellationToken: cancellationToken);
                            return true;
                        }
                    }
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (int.TryParse(messageText, out int numberHotel) && typePlace == "гостиница" && numberHotel >= 1 && numberHotel <= hotelList.Count)
                {
                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Hotels)
                    {
                        if (numberHotel == item.IdList) listId = item;
                    }
                    RouteCateringHotel hotel = new RouteCateringHotel();
                    hotel.IdHotel = listId.IdDB;
                    hotel.IdRoute = route.IdRoute;
                    if (!routeCateringHotels.Select(r => r.IdHotel).Contains(hotel.IdHotel))
                    {
                        routeCateringHotels.Add(hotel);
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект добавлен в маршрут!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(routeCateringHotels.Count);
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект уже добавлен в маршрут! Выберите другое заведение!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                }
                if (int.TryParse(messageText, out int numberSight) && typePlace == "достопримечательность" && numberSight >= 1 && numberSight <= sightList.Count)
                {
                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Sights)
                    {
                        if (numberSight == item.IdList) listId = item;
                    }
                    RouteEventSight sight = new RouteEventSight();
                    sight.IdSight = listId.IdDB;
                    sight.IdRoute = route.IdRoute;
                    if (!routeEventSight.Select(r => r.IdSight).Contains(sight.IdSight))
                    {
                        routeEventSight.Add(sight);
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект добавлен в маршрут!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект уже добавлен в маршрут! Выберите другое заведение!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                }
                if (int.TryParse(messageText, out int numberCatering) && typePlace == "место общепита" && numberCatering >= 1 && numberCatering <= cateringList.Count)
                {
                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Caterings)
                    {
                        if (numberCatering == item.IdList) listId = item;
                    }
                    RouteCateringHotel catering = new RouteCateringHotel();
                    catering.IdCatering = listId.IdDB;
                    catering.IdRoute = route.IdRoute;
                    if (!routeCateringHotels.Select(r => r.IdCatering).Contains(catering.IdCatering))
                    {
                        routeCateringHotels.Add(catering);
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект добавлен в маршрут!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект уже добавлен в маршрут! Выберите другое заведение!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                }
                if (int.TryParse(messageText, out int numberEvent) && typePlace == "мероприятие" && numberEvent >= 1 && numberEvent <= eventList.Count)
                {
                    GeneralService.ListId listId = new GeneralService.ListId();
                    foreach (var item in Events)
                    {
                        if (numberEvent == item.IdList) listId = item;
                    }
                    RouteEventSight _event = new RouteEventSight();
                    _event.IdEvent = listId.IdDB;
                    _event.IdRoute = route.IdRoute;
                    if (!routeEventSight.Select(r => r.IdEvent).Contains(_event.IdEvent))
                    {
                        routeEventSight.Add(_event);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        "Объект добавлен в маршрут!",
                        cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Объект уже добавлен в маршрут! Выберите другое заведение!",
                            cancellationToken: cancellationToken);
                        await botClient.SendTextMessageAsync(
                        chatId,
                        $"<b>Выберите, что вы хотите добавить:</b>\nДостопримечательности\nМеста общепита\nМероприятия\nГостиницы\n\nВернуться\nСохранить",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetCreateRouteKeyboard(),
                        cancellationToken: cancellationToken);
                        return true;
                    }
                }
                if (messageText.ToLower() == "сохранить")
                {
                    Console.WriteLine(routeEventSight.Count.ToString());
                    Console.WriteLine(route.NameRoute);
                    var routeSave = new Route();
                    routeSave.NameRoute = nameRoute;
                    routeSave.DateCreation = DateOnly.FromDateTime(DateTime.Now);
                    routeSave.IdUser = user.IdUser;
                    context.Routes.Add(routeSave);
                    context.SaveChanges();
                    Console.WriteLine(routeEventSight.Count.ToString());
                    foreach(var eventSight in routeEventSight)
                    {
                        eventSight.IdRoute = routeSave.IdRoute;
                        context.Add(eventSight);
                    }
                    foreach (var cateringHotels in routeCateringHotels)
                    {
                        cateringHotels.IdRoute = routeSave.IdRoute;
                        context.Add(cateringHotels);
                    }
                    context.SaveChanges();
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Маршрут добавлен!",
                        cancellationToken: cancellationToken);
                    await RouteAsync(botClient, chatId, cancellationToken);

                    string routeSaveInfo = "";
                    if (!string.IsNullOrEmpty(routeSave.NameRoute))
                    {
                        routeSaveInfo += $"\n\n{routeSave.NameRoute}";
                    }
                    var sights = context.RouteEventSights.Include(r => r.IdSightNavigation).Where(r => r.IdRoute == routeSave.IdRoute && r.IdSight != null);
                    if (sights.Count() != 0)
                    {
                        numberSight = 0;
                        routeSaveInfo += $"\nДостопримечательности:";
                        foreach (var sight in sights)
                        {
                            numberSight++;
                            var sightInfo = sight.IdSightNavigation;
                            routeSaveInfo += $"\n    {numberSight}. ";
                            if (!sightInfo.NameSight.ToLower().Contains(sightInfo.TypeSight.ToLower()))
                                routeSaveInfo += $"{sightInfo.TypeSight} {sightInfo.NameSight}";
                            else routeSaveInfo += $"{sightInfo.NameSight}";                            
                        }
                    }

                    var events = context.RouteEventSights.Include(r => r.IdEventNavigation).Where(r => r.IdRoute == routeSave.IdRoute && r.IdEvent != null);
                    if (events.Count() != 0)
                    {
                        numberEvent = 0;
                        routeSaveInfo += $"\nМероприятия:";
                        foreach (var _event in events)
                        {
                            numberEvent++;
                            var eventInfo = _event.IdEventNavigation;
                            routeSaveInfo += $"\n    {numberEvent}. ";
                            if (!eventInfo.NameEvent.ToLower().Contains(eventInfo.TypeEvent.ToLower()))
                                routeSaveInfo += $"{eventInfo.TypeEvent} {eventInfo.NameEvent}";
                            else routeSaveInfo += $"{eventInfo.NameEvent}\n";
                           
                        }
                    }

                    var caterings = context.RouteCateringHotels.Include(r => r.IdCateringNavigation).Where(r => r.IdRoute == routeSave.IdRoute && r.IdCatering != null);
                    if (caterings.Count() != 0)
                    {
                        numberCatering = 0;
                        routeSaveInfo += $"\nМеста общепита:";
                        foreach (var catering in caterings)
                        {
                            numberCatering++;
                            var cateringInfo = catering.IdCateringNavigation;
                            routeSaveInfo += $"\n    {numberCatering}. ";
                            if (!cateringInfo.EstablishmentName.ToLower().Contains(cateringInfo.EstablishmentCategory.ToLower()))
                                routeSaveInfo += $"{cateringInfo.EstablishmentCategory} {cateringInfo.EstablishmentName}";
                            else routeSaveInfo += cateringInfo.EstablishmentName;
                        }
                    }

                    var hotels = context.RouteCateringHotels.Include(r => r.IdHotelNavigation).Where(r => r.IdRoute == routeSave.IdRoute && r.IdHotel != null);
                    if (hotels.Count() != 0)
                    {
                        numberHotel = 0;
                        routeSaveInfo += $"\nГостиницы:";
                        foreach (var hotel in hotels)
                        {
                            numberHotel++;
                            var hotelInfo = hotel.IdHotelNavigation;
                            routeSaveInfo += $"\n    {numberHotel}. ";
                            if (!hotelInfo.HotelName.ToLower().Contains(hotelInfo.TypeHotel.ToLower()))
                                routeSaveInfo += $"{hotelInfo.TypeHotel} {hotelInfo.HotelName}";
                            else routeSaveInfo += hotelInfo.HotelName;
                        }
                    }

                    routeSaveInfo += $"\nМаршрут от пользователя с ID {user.IdUser}";
                    string subject = $"Создание нового индивидуального маршрута!";
                    nameRoute = "";
                    bool sent = await _emailService.SendEmailAsync(subject, routeSaveInfo);
                    return true;
                }
                if (messageText.ToLower() == "удалить маршрут")
                {
                    typePlace = "удалить маршрут";
                    if (string.IsNullOrEmpty(namesRoute))
                        namesRoute = "У вас нет маршрутов!";
                    await botClient.SendTextMessageAsync(
                        chatId,
                        $"Выберите маршрут, который хотите удалить:\n{namesRoute}",
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetDeleteRouteKeyboard(user),
                        cancellationToken: cancellationToken);
                    return true;
                }
                if (typePlace == "удалить маршрут" && messageText.ToLower() == "да")
                {
                    var routeDelete = context.Routes.Find(routeForDelete.IdRoute);
                    Console.WriteLine(routeForDelete.IdRoute);
                    var routeEventSights = context.RouteEventSights.Where(r => r.IdRouteNavigation == route).ToList();
                    foreach (var routeEventSight in routeEventSights)
                        context.Remove(routeEventSight);
                    var routeCateringHotels = context.RouteCateringHotels.Where(r => r.IdRouteNavigation == route).ToList();
                    foreach (var routeCateringHotel in routeCateringHotels)
                        context.Remove(routeCateringHotel);
                    context.SaveChanges();
                    context.Routes.Remove(routeDelete);
                    context.SaveChanges();
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "Маршрут удален!",
                        cancellationToken: cancellationToken);
                    typePlace = "";
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (typePlace == "удалить маршрут" && messageText.ToLower() == "нет")
                {
                    typePlace = "";
                    await RouteAsync(botClient, chatId, cancellationToken);
                    return true;
                }
                if (typePlace == "удалить маршрут")
                {
                    if(messageText.ToLower() != "назад")
                    {
                        var routes = context.Routes.Where(r => r.IdUserNavigation == user).ToList();
                        int numberRoute = 0;
                        foreach (var route in routes)
                        {
                            numberRoute++;
                            if (messageText == route.NameRoute || messageText.ToLower() == $"маршрут №{numberRoute}")
                            {
                                routeForDelete = route;
                            }
                        }
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Вы уверены, что хотите удалить маршрут?",
                            replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                            cancellationToken: cancellationToken);
                        return true;
                    }
                    else
                    {
                        typePlace = "";
                        await RouteAsync(botClient, chatId, cancellationToken);
                        return true;
                    }
                }
                
                
                
            }
            return false;
        }
    }
}