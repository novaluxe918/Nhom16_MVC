namespace Nhom16_MVC.Models.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // JWT Token
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string Avatar { get; set; }
        public string VaiTro { get; set; }
        public long SoDuTaiKhoan { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}