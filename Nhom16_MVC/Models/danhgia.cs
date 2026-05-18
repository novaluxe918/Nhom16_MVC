using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class danhgia
{
    public int madanhgia { get; set; }

    public int masanbong { get; set; }

    public int nguoithue { get; set; }

    public short diemso { get; set; }

    public string? binhluan { get; set; }

    public DateTime? thoigiandanhgia { get; set; }

    public virtual sanbong masanbongNavigation { get; set; } = null!;

    public virtual nguoidung nguoithueNavigation { get; set; } = null!;
}
