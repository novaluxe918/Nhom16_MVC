using System;
using System.Collections.Generic;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Models.Entities;

public partial class yeucauruttien
{
    public int mayeucau { get; set; }

    public int manguoidung { get; set; }

    public long sotien { get; set; }

    public DateTime? thoigianrut { get; set; }

    public string? magiaodich { get; set; }

    public string? mota { get; set; }

    public TrangThaiRutEnum trangthai { get; set; } = TrangThaiRutEnum.ChoXuLy;

    public string tennganhang { get; set; } = null!;

    public string sotaikhoan { get; set; } = null!;

    public virtual nguoidung manguoidungNavigation { get; set; } = null!;
}
