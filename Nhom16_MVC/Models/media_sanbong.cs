using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class media_sanbong
{
    public int mamedia { get; set; }

    public int masanbong { get; set; }

    public string loaimedia { get; set; } = null!;

    public string? ten { get; set; }

    public string link { get; set; } = null!;

    public string? mediaid { get; set; }

    public virtual sanbong masanbongNavigation { get; set; } = null!;
}
