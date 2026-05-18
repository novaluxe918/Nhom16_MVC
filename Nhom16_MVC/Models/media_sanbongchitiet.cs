using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class media_sanbongchitiet
{
    public int mamedia { get; set; }

    public int masanbongchitiet { get; set; }

    public string loaimedia { get; set; } = null!;

    public string? ten { get; set; }

    public string link { get; set; } = null!;

    public string? mediaid { get; set; }

    public virtual sanbongchitiet masanbongchitietNavigation { get; set; } = null!;
}
