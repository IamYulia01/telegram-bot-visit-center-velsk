using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class SpecialDaySight
{
    public int IdSpecialDaySight { get; set; }

    public int? SpecialDayDate { get; set; }

    public string? SpecialDayStatus { get; set; }

    public TimeOnly? StartWork { get; set; }

    public TimeOnly? EndWork { get; set; }
}
