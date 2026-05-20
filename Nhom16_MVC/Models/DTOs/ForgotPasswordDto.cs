using System.ComponentModel.DataAnnotations;

namespace Nhom16_MVC.Models.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }
    }

    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã xác thực không được để trống.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự trở lên.")]
        public string NewPassword { get; set; }
    }

    public class ResetPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}