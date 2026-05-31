using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Telegram_bot.Services.UserDoing
{
    public class EmailService
    {
        private readonly string _smptServer;
        private readonly int _smptPort;
        private readonly string _smptUsername;
        private readonly string _smptPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            _smptServer = "smpt.mail.ru";
            _smptPort = 587;
            _smptUsername = "vizittsentr.podderzhka.13@mail.ru";
            _smptPassword = "krvmSYugonh2BIdBoEhY";
        }
        public async Task<bool> SendEmailAsync(string subject, string body)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_smptUsername);
                    message.To.Add(new MailAddress("visitcentet@mail.ru"));
                    message.Subject = subject;
                    message.Body = body;
                    using (var client = new SmtpClient(_smptServer, _smptPort))
                    {
                        client.Credentials = new NetworkCredential(_smptUsername, _smptPassword);
                        client.EnableSsl = true;
                        await client.SendMailAsync(message);

                    }
                }
                Console.WriteLine("Email отправлен успешно");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке Email: {ex.Message}");
                return false;
            }
        }
    }
}
