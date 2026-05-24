using System.ComponentModel.DataAnnotations;

namespace Nhom16_MVC.Models.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(150)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [RegularExpression("^(nguoiThue|chuSan)$", ErrorMessage = "Vai trò chỉ có thể là 'nguoiThue' hoặc 'chuSan'")]
        public string VaiTro { get; set; } = "nguoiThue";
    }
}