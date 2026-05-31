using Telegram.Bot.Types.ReplyMarkups;
using Telegram_bot.Models;

namespace Telegram_bot.Services
{
    public class KeyboardService
    {
        public VisitCenterContext context {  get; set; }
        public KeyboardService() {
            context = new VisitCenterContext();
        }
        public ReplyKeyboardMarkup GetMainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Достопримечательности") },
                new[] { new KeyboardButton("Мероприятия"), new KeyboardButton("Гостиницы")},
                new[] { new KeyboardButton("Места общепита"), new KeyboardButton("Сувениры") },
                new[] { new KeyboardButton("Обратная связь") },
                new[] { new KeyboardButton("Индивидуальные маршруты") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        public ReplyKeyboardMarkup GetKeyboard(int count)
        {
            var buttons = new List<List<KeyboardButton>>();
            var rows = new List<KeyboardButton>();
            var hotels = context.Hotels.OrderBy(c => c.IdHotel).ToList();
            for (int i = 1; i <= count + 1;i++)
            {
                rows.Add(i.ToString());
                if (rows.Count == 4 || i == count)
                {
                    buttons.Add(new List<KeyboardButton>(rows));
                    rows.Clear();
                }
            }
            buttons.Add(new List<KeyboardButton> { new KeyboardButton("Назад") });
            return new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public ReplyKeyboardMarkup GetBackKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Назад") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetBackOrSkipKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Пропустить") },
                new[] { new KeyboardButton("Отменить") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetGenderKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Мужской"), new KeyboardButton("Женский")},
                new[] { new KeyboardButton("Пропустить") },
                new[] { new KeyboardButton("Отменить") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetUserProfileKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Создать анкету") },
                new[] { new KeyboardButton("Изменить анкету") },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetDeleteRouteKeyboard(Userbot user)
        {
            var routes = context.Routes.Where(r => r.IdUserNavigation == user).ToList();
            int numberRoute = 0;
            int count = routes.Count;
            var buttons = new List<List<KeyboardButton>>();
            var rows = new List<KeyboardButton>();
            var hotels = context.Hotels.OrderBy(c => c.IdHotel).ToList();
            foreach (var route in routes)
            {
                numberRoute++;
                if (string.IsNullOrEmpty(route.NameRoute))
                    rows.Add($"Маршрут №{numberRoute}");
                else rows.Add(route.NameRoute);
                if (rows.Count == 2 || numberRoute == count)
                {
                    buttons.Add(new List<KeyboardButton>(rows));
                    rows.Clear();
                }
            }
            buttons.Add(new List<KeyboardButton> { new KeyboardButton("Назад") });
            return new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
            
        }
        public ReplyKeyboardMarkup GetSkipKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Пропустить") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetAddKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Пропустить") },
                new[] { new KeyboardButton("Назад") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetConfirmationKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Да"), new KeyboardButton("Нет") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetToSightKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("К достопримечательностям") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetToHotelKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("К гостиницам") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetToCateringKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("К местам общепита") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public ReplyKeyboardMarkup GetRouteKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Создать маршрут") },
                new[] { new KeyboardButton("Удалить маршрут") },
                new[] { new KeyboardButton("В главное меню") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetCreateRouteKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Достопримечательность") },
                new[] { new KeyboardButton("Место общепита") },
                new[] { new KeyboardButton("Мероприятие"), new KeyboardButton("Гостиница") },
                new[] { new KeyboardButton("Вернуться"), new KeyboardButton("Сохранить") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public ReplyKeyboardMarkup GetFeedbackKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Отправить") },
                new[] { new KeyboardButton("Отменить") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public ReplyKeyboardMarkup GetSaveOrCancelKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Сохранить"), new KeyboardButton("Отменить") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        
    }

}