using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class chitietdatsan
{
    public int machitietdatsan { get; set; }

    public int madatsan { get; set; }

    public int masanchitiet { get; set; }

    public int? maloaidat { get; set; }

    public TimeOnly giobatdau { get; set; }

    public TimeOnly gioketthuc { get; set; }

    public bool? covande { get; set; }

    public string? trangthaidatsan { get; set; }

    public virtual datsan madatsanNavigation { get; set; } = null!;

    public virtual loaihinhdat? maloaidatNavigation { get; set; }

    public virtual sanbongchitiet masanchitietNavigation { get; set; } = null!;
}
