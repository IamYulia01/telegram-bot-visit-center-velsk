﻿using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class Souvenir
{
    public int IdSouvenir { get; set; }

    public string Product { get; set; } = null!;

    public string NameSouvenir { get; set; } = null!;

    public string? Tastes { get; set; }

    public string? Weight { get; set; }
}
