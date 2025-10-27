using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class OperatingMode
{
    public int IdOperatingMode { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? DayOfWeek { get; set; }

    public virtual ICollection<SightOperatingMode> SightOperatingModes { get; set; } = new List<SightOperatingMode>();
}
