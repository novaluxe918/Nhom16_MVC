using System;
using System.Collections.Generic;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Models.Entities;

public partial class chitietdatsan
{
    public int machitietdatsan { get; set; }

    public int madatsan { get; set; }

    public int masanchitiet { get; set; }

    public int? maloaidat { get; set; }

    public DateTime giobatdau { get; set; }

    public DateTime gioketthuc { get; set; }

    public bool? covande { get; set; }

    public TrangThaiDatEnum trangthaidatsan { get; set; } = TrangThaiDatEnum.ChoXacNhan;

    public virtual datsan madatsanNavigation { get; set; } = null!;

    public virtual loaihinhdat? maloaidatNavigation { get; set; }

    public virtual sanbongchitiet masanchitietNavigation { get; set; } = null!;
}
