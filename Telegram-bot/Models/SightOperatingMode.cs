using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class SightOperatingMode
{
    public int IdSightOperatingMode { get; set; }

    public int IdSight { get; set; }

    public int IdOperatingMode { get; set; }

    public virtual OperatingMode IdOperatingModeNavigation { get; set; } = null!;

    public virtual Sight IdSightNavigation { get; set; } = null!;
}
