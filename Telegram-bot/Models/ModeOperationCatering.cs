using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class ModeOperationCatering
{
    public int IdModeOperationCatering { get; set; }

    public string WorkingDayWeek { get; set; } = null!;

    public TimeOnly Beginning { get; set; }

    public TimeOnly EndDay { get; set; }

    public virtual ICollection<CateringModeOperationCatering> CateringModeOperationCaterings { get; set; } = new List<CateringModeOperationCatering>();
}
