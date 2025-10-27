using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class Feedback
{
    public int IdFeedback { get; set; }

    public string? MessageSubject { get; set; }

    public string TextMessage { get; set; } = null!;

    public string ContactCommunicationNumber { get; set; } = null!;

    public int IdUser { get; set; }

    public virtual Userbot IdUserNavigation { get; set; } = null!;
}
