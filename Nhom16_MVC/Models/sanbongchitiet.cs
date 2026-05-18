using System;
using System.Collections.Generic;

namespace Nhom16_MVC.Models;

public partial class sanbongchitiet
{
    public int masanchitiet { get; set; }

    public int masanbong { get; set; }

    public int maloaisan { get; set; }

    public string tensanchitiet { get; set; } = null!;

    public long giathuebuoisang { get; set; }

    public long giathuebuoitoi { get; set; }

    public virtual ICollection<chitietdatsan> chitietdatsan { get; set; } = new List<chitietdatsan>();

    public virtual loaisan maloaisanNavigation { get; set; } = null!;

    public virtual sanbong masanbongNavigation { get; set; } = null!;

    public virtual ICollection<media_sanbongchitiet> media_sanbongchitiet { get; set; } = new List<media_sanbongchitiet>();
}
