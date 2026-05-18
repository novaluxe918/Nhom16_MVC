using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class yeucauruttien
{
    public int mayeucau { get; set; }

    public int manguoidung { get; set; }

    public long sotien { get; set; }

    public DateTime? thoigianrut { get; set; }

    public string? magiaodich { get; set; }

    public string? mota { get; set; }

    public string? trangthai { get; set; }

    public string tennganhang { get; set; } = null!;

    public string sotaikhoan { get; set; } = null!;

    public virtual nguoidung manguoidungNavigation { get; set; } = null!;
}
