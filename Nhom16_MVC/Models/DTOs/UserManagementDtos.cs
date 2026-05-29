using System.Collections.Generic;

namespace Nhom16_MVC.Models.DTOs
{
    public class UserDto
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string VaiTro { get; set; } = string.Empty;
        public long SoDuTaiKhoan { get; set; }
        public bool IsEmailVerified { get; set; }
        public string TrangThai { get; set; } = "hoat_dong";
    }

    public class UserManagementResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<UserDto>? Data { get; set; }
    }

    public class ToggleLockDto
    {
        public int MaNguoiDung { get; set; }
        public bool IsLocked { get; set; } // Giữ nguyên để Postman truyền true (khóa) / false (mở) cho tiện
        public string LockReasonType { get; set; } = string.Empty;
        public string? CustomReason { get; set; }
    }
}