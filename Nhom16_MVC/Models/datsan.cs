using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class datsan
{
    public int madatsan { get; set; }

    public int nguoithue { get; set; }

    public DateOnly ngaydat { get; set; }

    public DateTime? ngaythanhtoan { get; set; }

    public virtual ICollection<chitietdatsan> chitietdatsan { get; set; } = new List<chitietdatsan>();

    public virtual nguoidung nguoithueNavigation { get; set; } = null!;
}
