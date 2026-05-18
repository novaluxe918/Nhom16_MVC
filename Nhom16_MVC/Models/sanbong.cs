using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class sanbong
{
    public int masanbong { get; set; }

    public int chusan { get; set; }

    public string tensan { get; set; } = null!;

    public string? mota { get; set; }

    public string? hinhanh { get; set; }

    public bool? daduyet { get; set; }

    public string? diachi { get; set; }

    public string? quan { get; set; }

    public string? huyen { get; set; }

    public string? xa { get; set; }

    public string? thanhpho { get; set; }

    public decimal? kinhdo { get; set; }

    public decimal? vido { get; set; }

    public DateTime? createdat { get; set; }

    public virtual nguoidung chusanNavigation { get; set; } = null!;

    public virtual ICollection<danhgia> danhgia { get; set; } = new List<danhgia>();

    public virtual ICollection<media_sanbong> media_sanbong { get; set; } = new List<media_sanbong>();

    public virtual ICollection<sanbongchitiet> sanbongchitiet { get; set; } = new List<sanbongchitiet>();
}
