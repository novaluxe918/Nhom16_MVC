using System.ComponentModel.DataAnnotations;

namespace Nhom16_MVC.Models.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email hoặc Số điện thoại là bắt buộc")]
        public string LoginId { get; set; } // Email hoặc SĐT

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MatKhau { get; set; }
    }
}