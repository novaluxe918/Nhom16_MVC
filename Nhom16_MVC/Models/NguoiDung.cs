using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom16_MVC.Models
{
    [Table("nguoidung")]
    public class NguoiDung
    {
        [Key]
        [Column("manguoidung")]
        public int MaNguoiDung { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("hoten")]
        public string HoTen { get; set; }

        [Column("avatar")]
        public string? Avatar { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [MaxLength(15)]
        [Column("sodienthoai")]
        public string? SoDienThoai { get; set; }

        [Required]
        [Column("matkhau")]
        public string MatKhau { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("vaitro")]
        public string VaiTro { get; set; } = "nguoiThue";

        [Column("sodutaikhoan")]
        public long SoDuTaiKhoan { get; set; } = 0;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Các cột mới cho xác thực email
        [Column("isemailverified")]
        public bool IsEmailVerified { get; set; } = false;

        [Column("verificationtoken")]
        public string? VerificationToken { get; set; }

        [Column("tokenexpiry")]
        public DateTime? TokenExpiry { get; set; }
    }
}