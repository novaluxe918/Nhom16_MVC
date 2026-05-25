using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models.Entities;

public partial class chat
{
    public int matinnhan { get; set; }

    public int nguoigui { get; set; }

    public int nguoinhan { get; set; }

    public string noidung { get; set; } = null!;

    public DateTime? thoigiangui { get; set; }

    public bool daDoc { get; set; } = false;

    public virtual nguoidung nguoiguiNavigation { get; set; } = null!;

    public virtual nguoidung nguoinhanNavigation { get; set; } = null!;
}
