using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_bot.Services
{
    public class KeyboardService
    {
        public ReplyKeyboardMarkup GetMainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Достопримечательности") },
                new[] { new KeyboardButton("Мероприятия") },
                new[] { new KeyboardButton("Гостиницы") },
                new[] { new KeyboardButton("Места общепита") },
                new[] { new KeyboardButton("Сувениры") },
                new[] { new KeyboardButton("Анкета") },
                new[] { new KeyboardButton("Индивидуальные маршруты") },
                new[] { new KeyboardButton("Обратная связь") },
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetSightKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] {
                    new KeyboardButton("5"),
                    new KeyboardButton("6"),
                    new KeyboardButton("7"),
                    new KeyboardButton("8")
                },
                new[] {
                    new KeyboardButton("9"),
                    new KeyboardButton("10"),
                    new KeyboardButton("11"),
                    new KeyboardButton("12")
                },
                new[] {
                    new KeyboardButton("13"),
                    new KeyboardButton("14"),
                    new KeyboardButton("15"),
                    new KeyboardButton("16")
                },
                new[] { new KeyboardButton("Особые дни") },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetSightDeveloperKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] {
                    new KeyboardButton("5"),
                    new KeyboardButton("6"),
                    new KeyboardButton("7"),
                    new KeyboardButton("8")
                },
                new[] {
                    new KeyboardButton("9"),
                    new KeyboardButton("10"),
                    new KeyboardButton("11"),
                    new KeyboardButton("12")
                },
                new[] {
                    new KeyboardButton("13"),
                    new KeyboardButton("14"),
                    new KeyboardButton("15"),
                    new KeyboardButton("16")
                },
                new[] { new KeyboardButton("Назад") }

            })
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
        public ReplyKeyboardMarkup GetHotelKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] {
                    new KeyboardButton("5"),
                    new KeyboardButton("6"),
                    new KeyboardButton("7"),
                    new KeyboardButton("8")
                },
                new[] {
                    new KeyboardButton("9"),
                    new KeyboardButton("10"),
                    new KeyboardButton("11"),
                    new KeyboardButton("12")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetCateringKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] {
                    new KeyboardButton("5"),
                    new KeyboardButton("6"),
                    new KeyboardButton("7"),
                    new KeyboardButton("8")
                },
                new[] {
                    new KeyboardButton("9"),
                    new KeyboardButton("10"),
                    new KeyboardButton("11"),
                    new KeyboardButton("12")
                },
                new[] {
                    new KeyboardButton("13"),
                    new KeyboardButton("14")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetEventListKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetSouvenirKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetSpecialDayKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        public ReplyKeyboardMarkup GetModeOperationKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public ReplyKeyboardMarkup GetTicketKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                    new KeyboardButton("4")
                },
                new[] { new KeyboardButton("Назад") }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
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
        public ReplyKeyboardMarkup GetDeleteRouteKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("1 маршрут") },
                new[] { new KeyboardButton("2 маршрут") },
                new[] { new KeyboardButton("3 маршрут") },

                new[] { new KeyboardButton("Вернуться") }

            })
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
        public ReplyKeyboardMarkup GetEditUserProfileKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Фамилия"), new KeyboardButton("Имя") },
                new[] { new KeyboardButton("Отчество"), new KeyboardButton("Номер телефона") },
                new[] { new KeyboardButton("Пол"), new KeyboardButton("Дата рождения") },
                new[] {new KeyboardButton("Сохранить"), new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
        /*
        public ReplyKeyboardMarkup GetCalendarKeyboard()
        {
        }
        */
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
                new[] { new KeyboardButton("Мероприятие") },
                new[] { new KeyboardButton("Место общепита") },
                new[] { new KeyboardButton("Гостиница") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
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

        public ReplyKeyboardMarkup GetMainMenuDeveloperKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Управление достопримечательностями") },
                new[] { new KeyboardButton("Управление мероприятиями") },
                new[] { new KeyboardButton("Управление гостиницами") },
                new[] { new KeyboardButton("Управление местами общепита") },
                new[] { new KeyboardButton("Управление сувенирами") },
                new[] { new KeyboardButton("Управление особыми днями") },
                new[] { new KeyboardButton("Управление рабочими графиками") },
                new[] { new KeyboardButton("Управление билетами") },
                new[] { new KeyboardButton("Выйти") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetSpecialDaysDeveloperKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Особые дни достопримечательности") },
                new[] { new KeyboardButton("Особые дни общепита") },
                new[] { new KeyboardButton("Назад") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetDoingDeveloperKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Добавить") },
                new[] { new KeyboardButton("Изменить") },
                new[] { new KeyboardButton("Удалить") },
                new[] { new KeyboardButton("Назад") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetOperatingModeDeveloperKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Pабочие графики достопримечательностей") },
                new[] { new KeyboardButton("Pабочие графики общепита") },
                new[] { new KeyboardButton("Назад") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditSouvenirKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Название сувенира") },
                new[] { new KeyboardButton("Продукт") },
                new[] { new KeyboardButton("Вкус") },
                new[] { new KeyboardButton("Вес") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditSpecialDayKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Дата") },
                new[] { new KeyboardButton("Статус дня") },
                new[] { new KeyboardButton("Время начала работы") },
                new[] { new KeyboardButton("Время окончания работы") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditEventKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Название") },
                new[] { new KeyboardButton("Тип") },
                new[] { new KeyboardButton("Дата проведения") },
                new[] { new KeyboardButton("Время начала") },
                new[] { new KeyboardButton("Возрастное ограничение") },
                new[] { new KeyboardButton("Улица проведения") },
                new[] { new KeyboardButton("Дом проведения") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditHotelKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("url гостиницы") },
                new[] { new KeyboardButton("Название") },
                new[] { new KeyboardButton("Контактный номер") },
                new[] { new KeyboardButton("Улица гостиницы") },
                new[] { new KeyboardButton("Дом гостиницы") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditCateringKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("url общепита") },
                new[] { new KeyboardButton("Название") },
                new[] { new KeyboardButton("Категория") },
                new[] { new KeyboardButton("Контактный номер") },
                new[] { new KeyboardButton("Улица общепита") },
                new[] { new KeyboardButton("Дом общепита") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditSightKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("url достопримечательности") },
                new[] { new KeyboardButton("Название") },
                new[] { new KeyboardButton("Тип") },
                new[] { new KeyboardButton("Электронная почта") },
                new[] { new KeyboardButton("Контактный номер") },
                new[] { new KeyboardButton("Количество мест") },
                new[] { new KeyboardButton("Описание") },
                new[] { new KeyboardButton("Улица расположения") },
                new[] { new KeyboardButton("Дом расположения") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
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
        public ReplyKeyboardMarkup GetEditOperationModeKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("День недели") },
                new[] { new KeyboardButton("Начало рабочего дня") },
                new[] { new KeyboardButton("Конец рабочего дня") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("nВернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetEditTicketKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Мероприятие") },
                new[] { new KeyboardButton("Минимальный возраст") },
                new[] { new KeyboardButton("Максимальный возраст") },
                new[] { new KeyboardButton("Цена") },
                new[] { new KeyboardButton("Сохранить") },
                new[] { new KeyboardButton("Вернуться") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        public ReplyKeyboardMarkup GetDayOWeekKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Понедельник") },
                new[] { new KeyboardButton("Вторник") },
                new[] { new KeyboardButton("Среда") },
                new[] { new KeyboardButton("Четверг") },
                new[] { new KeyboardButton("Пятница") },
                new[] { new KeyboardButton("Суббота") },
                new[] { new KeyboardButton("Воскресенье") },
                new[] { new KeyboardButton("Назад") }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
    }

}