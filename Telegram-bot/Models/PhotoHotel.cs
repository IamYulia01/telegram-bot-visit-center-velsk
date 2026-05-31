using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_bot;
public partial class PhotoHotel
{
    public int IdPhotoHotel { get; set; }

    public string NameFile { get; set; } = null!;

    public string? DescriptionPhoto { get; set; }

    public int IdHotel { get; set; }

    public virtual Hotel IdHotelNavigation { get; set; } = null!;
}
