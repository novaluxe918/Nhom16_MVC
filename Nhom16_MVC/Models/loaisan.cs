using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class loaisan
{
    public int maloaisan { get; set; }

    public string tenloaisan { get; set; } = null!;

    public virtual ICollection<sanbongchitiet> sanbongchitiet { get; set; } = new List<sanbongchitiet>();
}
