using System;

namespace Nhom16_MVC.Models.DTOs
{
    /// <summary>
    /// DTO hiển thị danh sách các yêu cầu rút tiền đang chờ duyệt lên màn hình Admin
    /// </summary>
    public class AdminWithdrawalRequestViewDto
    {
        public int MaYeuCau { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public decimal SoTienRut { get; set; }
        public string TrangThai { get; set; } = string.Empty; // "dang_cho", "da_chuyen", "tu_choi"
        public string ThongTinNganHang { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Body data gửi từ Admin lên để xử lý Đạt hoặc Từ chối lệnh rút tiền
    /// </summary>
    public class ProcessWithdrawalRequest
    {
        public int MaYeuCau { get; set; }
        public string TrangThaiMoi { get; set; } = string.Empty; // "da_chuyen" hoặc "tu_choi"
        public string LyDoTuChoi { get; set; } = string.Empty; // Bắt buộc nếu là "tu_choi"
    }

    /// <summary>
    /// Kết quả phản hồi chuẩn của API quản lý tài chính
    /// </summary>
    public class FinancialManagementResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}