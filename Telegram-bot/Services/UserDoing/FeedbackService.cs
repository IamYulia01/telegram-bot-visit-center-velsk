using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot.Models;
using Telegram_bot.Services;
using Telegram_bot.Services.UserDoing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Telegram_bot.Services
{
    public class FeedbackService
    {
        private readonly KeyboardService _keyboardService;
        private readonly StateService _stateService;
        private readonly EmailService _emailService;

        public string message { get; set; }
        public static int step { get; set; }
        public Userbot user { get; set; }
        public static Feedback feedback = new Feedback();
        public VisitCenterContext context { get; set; }

        public FeedbackService(KeyboardService keyboardService, StateService stateService)
        {
            _keyboardService = keyboardService;
            _stateService = stateService;
            context = new VisitCenterContext();
            _emailService = new EmailService();
        }

        private async Task FeedbackAsync(ITelegramBotClient botClient, ChatId chatId, Telegram.Bot.Types.Update update,  CancellationToken cancellationToken)
        {
            step = 0;
            await botClient.SendTextMessageAsync(
                chatId,
                "Для отправки обратной связи отрпавьте необходимую информацию!",
                replyMarkup: _keyboardService.GetBackKeyboard(),
                cancellationToken: cancellationToken);
            await botClient.SendTextMessageAsync(
                chatId,
                "Введите тему сообщения",
                cancellationToken: cancellationToken);
            
        }

        public async Task<bool> TryHandleMessageAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return false;

            if (message.Text is not { } messageText)
                return false;
            

            var chatId = message.Chat.Id;
            var currentSection = _stateService.GetUserSection(chatId);
            user = context.Userbots.Find(chatId);
            if (messageText.ToLower() == "обратная связь")
            {
                _stateService.SetUserSection(chatId, "feedback");
                await FeedbackAsync(botClient, chatId, update, cancellationToken);
                return true;
            }
            if (currentSection != "feedback")
                return false;
            if (messageText.ToLower() == "назад")
            {
                GeneralService.MainMenuShow(botClient, chatId, cancellationToken, _keyboardService);
                return true;
            }
            switch (step)
            {
                case 0:
                    {
                        feedback.MessageSubject = messageText;
                        step = 1;
                        Console.WriteLine(step);
                        await botClient.SendTextMessageAsync(
                            chatId,
                            $"Введите текст сообщения",
                            cancellationToken: cancellationToken);
                        return true;
                    }
                case 1:
                    {
                        feedback.TextMessage = messageText;
                        step = 2;
                        await botClient.SendTextMessageAsync(
                            chatId,
                            "Введите ваш контактный номер",
                            cancellationToken: cancellationToken);
                        return true;
                    }
                case 2:
                    {
                        string numder = messageText.Replace("-", "");
                        numder = numder.Replace("+", "");
                        numder = numder.Replace("(", "");
                        numder = numder.Replace(")", "");
                        numder = numder.Replace(" ", "");
                        if (numder.Length <= 15)
                        {
                            feedback.ContactCommunicationNumber = numder;
                        }
                        else
                        {
                            step = 2;
                            await botClient.SendTextMessageAsync(
                            chatId,
                            "Слишком много символов! Введите ваш контактный номер заново или пропустите",
                            cancellationToken: cancellationToken);
                            return true;
                        }
                            step = 3;
                        string info = $"  <b>Тема сообщения:</b> {feedback.MessageSubject}\n  <b>Текст сообщения:</b> {feedback.TextMessage}\n  <b>Номер для связи:</b> {feedback.ContactCommunicationNumber}\n";
                        await botClient.SendTextMessageAsync(
                            chatId,
                            $"Вот письмо, которое будет отправлено: \n\n{info}\n\nВы уверены, что хотите отправить обратную связь?",
                            replyMarkup: _keyboardService.GetConfirmationKeyboard(),
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);
                        return true;

                    }
                case 3:
                    {
                        if (messageText.ToLower() == "да")
                        {
                            feedback.IdUserNavigation = user;
                            context.Feedbacks.Add(feedback);
                            context.SaveChanges();
                            await SendFeedbackToEmailAsync(feedback, chatId);
                            await botClient.SendTextMessageAsync(
                                chatId,
                                "Обратная связь отправлена!",
                                cancellationToken: cancellationToken);
                            await botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:\nДостопримечательности\nМероприятия\nГостиницы\nМеста общепита\nСувениры\nАнкета\nИндивидуальные маршруты\nОбратная связь",
                                replyMarkup: _keyboardService.GetMainMenuKeyboard(),
                                cancellationToken: cancellationToken);
                            return true;
                        }
                        if (messageText.ToLower() == "нет")
                        {
                            await botClient.SendTextMessageAsync(
                                chatId,
                                "Обратная связь не отправлена!",
                                cancellationToken: cancellationToken);
                            await botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:\nДостопримечательности\nМероприятия\nГостиницы\nМеста общепита\nСувениры\nАнкета\nИндивидуальные маршруты\nОбратная связь",
                                replyMarkup: _keyboardService.GetMainMenuKeyboard(),
                                cancellationToken: cancellationToken);
                            return true;
                        }
                        return true;
                    }
            
            }
            
            return false;
        }
        public async Task SendFeedbackToEmailAsync(Feedback feedback, ChatId chatId)
        {
            string subject = $"Новая обратная связь от пользователя: {feedback.MessageSubject}";
            string body = $"{feedback.TextMessage}\n\nКонтактный номер: {feedback.ContactCommunicationNumber}";
            bool sent = await _emailService.SendEmailAsync(subject, body);
            if (sent) Console.WriteLine("Сообщение отправлено успешно");
            else Console.WriteLine("Сообщение не отправлено");
        }
    }
}