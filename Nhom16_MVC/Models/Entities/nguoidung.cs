using System;
using System.Collections.Generic;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Models.Entities;

public partial class nguoidung
{
    public int manguoidung { get; set; }

    public string hoten { get; set; } = null!;

    public string? avatar { get; set; }

    public string email { get; set; } = null!;

    public string? sodienthoai { get; set; }

    public string matkhau { get; set; } = null!;

    public VaiTroEnum vaitro { get; set; }

    public long sodutaikhoan { get; set; }

    public DateTime? createdat { get; set; }

    public bool? isemailverified { get; set; }

    // Sử dụng trường này làm OTP khôi phục/xác thực mật khẩu (Khớp với DB)
    public string? verificationtoken { get; set; }

    // Sử dụng trường này làm thời hạn của OTP (Khớp với DB)
    public DateTime? tokenexpiry { get; set; }

    // Đã xóa bỏ hoàn toàn resetToken và resetTokenExpiry để tránh lỗi DbUpdateException

    public virtual ICollection<chat> chatnguoiguiNavigation { get; set; } = new List<chat>();

    public virtual ICollection<chat> chatnguoinhanNavigation { get; set; } = new List<chat>();

    public virtual ICollection<danhgia> danhgia { get; set; } = new List<danhgia>();

    public virtual ICollection<datsan> datsan { get; set; } = new List<datsan>();

    public virtual ICollection<naptien> naptien { get; set; } = new List<naptien>();

    public virtual ICollection<sanbong> sanbong { get; set; } = new List<sanbong>();

    public virtual ICollection<yeucauruttien> yeucauruttien { get; set; } = new List<yeucauruttien>();
}