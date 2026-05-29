using System;

namespace Nhom16_MVC.Models.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin chi tiết của đánh giá lên giao diện quản trị Admin
    /// </summary>
    public class AdminRatingViewDto
    {
        public int MaDanhGia { get; set; }
        public int MaSanBong { get; set; }
        public string TenSan { get; set; } = string.Empty;
        public string TenNguoiDung { get; set; } = string.Empty;
        public int SoSao { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request gửi lên khi Admin muốn xóa một đánh giá spam, xúc phạm
    /// </summary>
    public class DeleteRatingRequest
    {
        public int MaDanhGia { get; set; }
    }

    /// <summary>
    /// Phản hồi kết quả xử lý của API quản lý đánh giá
    /// </summary>
    public class RatingManagementResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}