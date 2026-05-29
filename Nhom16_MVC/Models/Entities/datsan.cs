using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models.Entities;

public partial class datsan
{
    public int madatsan { get; set; }

    public int nguoithue { get; set; }

     public DateTime ngaydat { get; set; }

    public DateTime? ngaythanhtoan { get; set; }


    public long sotienthanhtoan { get; set; }

    public virtual ICollection<chitietdatsan> chitietdatsan { get; set; } = new List<chitietdatsan>();

    public virtual nguoidung nguoithueNavigation { get; set; } = null!;
}
