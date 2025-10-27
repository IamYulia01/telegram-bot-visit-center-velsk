using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class SpecialDayCatering
{
    public int IdSpecialDayCatering { get; set; }

    public int? Date { get; set; }

    public string? StatusDay { get; set; }

    public TimeOnly? TimeStartWork { get; set; }

    public TimeOnly? TimeEndWork { get; set; }
}
