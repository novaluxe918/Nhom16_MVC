using System;
using System.Collections.Generic;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Models.Entities;

public partial class naptien
{
    public int manaptien { get; set; }

    public int nguoinap { get; set; }

    public long sotien { get; set; }

    public DateTime? thoigiannap { get; set; }

    public string? magiaodich { get; set; }

    public PhuongThucNapEnum? phuongthuc { get; set; } = PhuongThucNapEnum.VNPay;

    public TrangThaiNapEnum trangthai { get; set; } = TrangThaiNapEnum.ChoXuLy;

    public virtual nguoidung nguoinapNavigation { get; set; } = null!;
}
