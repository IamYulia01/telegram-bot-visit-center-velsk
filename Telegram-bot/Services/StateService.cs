using System.Collections.Concurrent;

namespace Telegram_bot.Services
{
    public class StateService
    {
        private readonly ConcurrentDictionary<long, string> _userSections = new();

        public void SetUserSection(long chatId, string section)
        {
            _userSections[chatId] = section;
        }

        public string GetUserSection(long chatId)
        {
            return _userSections.TryGetValue(chatId, out var section) ? section : "main";
        }

        public void ResetUserSection(long chatId)
        {
            _userSections.TryRemove(chatId, out _);
        }
    }
}