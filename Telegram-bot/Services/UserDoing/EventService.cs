using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;

namespace Telegram_bot.Services
{
    public class EventService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        public VisitCenterContext context { get; set; }
        public List<Event> eventList { get; set; }

        public EventService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            context = new VisitCenterContext();
            eventList = context.Events.Where(p => p.DateEvent >= DateOnly.FromDateTime(DateTime.Now) || p.DateEvent == null).OrderBy(c => c.IdEvent).ToList();
        }

        private async Task EventAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
        {
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
                if(eventFuture.DateEvent != null)
                    events += $"\n<b>Дата проведения:</b> {eventFuture.DateEvent}";
                if (!string.IsNullOrEmpty(eventFuture.AgeLimit))
                    events += $"\n<b>Возрастное ограничение:</b> <i>{eventFuture.AgeLimit}</i>";
                if(tickets != null && tickets.Any())
                {
                    events += $"\n<b>Билеты:</b>";
                    foreach(var ticket in tickets)
                    {
                        events += "\n    ";
                        if (ticket.MinimumAge != null && ticket.MinimumAge > 0 && ticket.MaximumAge != null && ticket.MaximumAge < 100)
                            events += $"<b>От {ticket.MinimumAge} до {ticket.MaximumAge} лет:</b>";
                        else if ((ticket.MinimumAge == null || ticket.MinimumAge <= 0) && ticket.MaximumAge != null && ticket.MaximumAge < 100)
                            events += $"<b>До {ticket.MaximumAge} лет:</b>";
                        else if (ticket.MinimumAge != null && ticket.MinimumAge > 0 && (ticket.MaximumAge == null || ticket.MaximumAge >= 100))
                            events += $"<b>От {ticket.MinimumAge} лет:</b>";
                        if(ticket.Price != 0 && ticket.Price != null)
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
                $"Вот доступные мероприятия: \n\n{events}",
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
            if (currentSection == "event" || messageText.ToLower() == "мероприятия")
            {
                if (messageText.ToLower() == "мероприятия")
                {
                    _stateService.SetUserSection(chatId, "event");
                    await EventAsync(botClient, chatId, cancellationToken);
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