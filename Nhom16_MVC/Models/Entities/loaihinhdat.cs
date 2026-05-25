using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models.Entities;

public partial class loaihinhdat
{
    public int maloaidat { get; set; }

    public string tenloaidat { get; set; } = null!;

    public virtual ICollection<chitietdatsan> chitietdatsan { get; set; } = new List<chitietdatsan>();
}
