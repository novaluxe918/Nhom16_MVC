using System.ComponentModel.DataAnnotations;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Models.DTOs
{
    /// <summary>
    /// DTO để đăng ký tài khoản mới
    /// </summary>
    public class AuthDTOs
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 150 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Số điện thoại phải từ 10 đến 15 chữ số")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò không được để trống")]
        public string VaiTro { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO để xác thực email qua mã OTP
    /// </summary>
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP không được để trống")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải là 6 chữ số")]
        public string OTP { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO để quên mật khẩu (bước 1: nhập email)
    /// </summary>
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO để đặt lại mật khẩu (bước 2: nhập OTP + mật khẩu mới)
    /// </summary>
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP không được để trống")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải là 6 chữ số")]
        public string OTP { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự trở lên")]
        public string MatKhauMoi { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO để đăng nhập hệ thống
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string MatKhau { get; set; } = string.Empty;
    }
}