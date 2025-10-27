using System;
using System.Collections.Generic;

namespace Telegram_bot;

public partial class Hotel
{
    public int IdHotel { get; set; }

    public string HotelName { get; set; } = null!;

    public string HotelStreet { get; set; } = null!;

    public string HotelHouse { get; set; } = null!;

    public string ContactNumberHotel { get; set; } = null!;

    public string? HotelUrl { get; set; }

    public virtual ICollection<RouteCateringHotel> RouteCateringHotels { get; set; } = new List<RouteCateringHotel>();
}
