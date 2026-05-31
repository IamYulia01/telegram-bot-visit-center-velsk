using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class Userbot
{
    public long IdUser { get; set; }

    public string? UserName { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
}
