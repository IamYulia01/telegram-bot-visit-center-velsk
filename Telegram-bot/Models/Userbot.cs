using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class Userbot
{
    public int IdUser { get; set; }

    public string? UserName { get; set; }

    public string? LastName { get; set; }

    public string? NamePerson { get; set; }

    public string? Patronymic { get; set; }

    public string? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? DateBirth { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
}
