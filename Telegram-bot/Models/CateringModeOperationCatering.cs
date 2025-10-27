using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class CateringModeOperationCatering
{
    public int IdCateringModeOperationCatering { get; set; }

    public int IdModeOperationCatering { get; set; }

    public int IdCatering { get; set; }

    public virtual Catering IdCateringNavigation { get; set; } = null!;

    public virtual ModeOperationCatering IdModeOperationCateringNavigation { get; set; } = null!;
}
