using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class naptien
{
    public int manaptien { get; set; }

    public int nguoinap { get; set; }

    public long sotien { get; set; }

    public DateTime? thoigiannap { get; set; }

    public string? magiaodich { get; set; }

    public string? trangthai { get; set; }

    public virtual nguoidung nguoinapNavigation { get; set; } = null!;
}
