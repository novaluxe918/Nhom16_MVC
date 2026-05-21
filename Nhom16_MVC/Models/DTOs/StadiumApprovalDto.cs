using System.Collections.Generic;

namespace Nhom16_MVC.Models.DTOs
{
    // DTO dùng để hiển thị danh sách sân chờ duyệt kèm mảng ảnh chi tiết
    public class StadiumApprovalViewDto
    {
        public int MaSanBong { get; set; }
        public string TenSan { get; set; }
        public string ChuSan { get; set; }
        public string DiaChi { get; set; }
        public string MoTa { get; set; }
        public string HinhAnhDaiDien { get; set; }
        public List<string> DanhSachHinhAnhChiTiet { get; set; } = new List<string>();
        public bool DaDuyet { get; set; }
    }

    // Request gửi lên từ giao diện Admin khi bấm nút Duyệt/Từ chối
    public class ApproveStadiumRequest
    {
        public long MaSanBong { get; set; }
        public bool IsApproved { get; set; } // true = Duyệt, false = Từ chối
        public string LyDoTuChoi { get; set; } // Bắt buộc nhập nếu IsApproved = false
    }

    // Phản hồi kết quả API
    public class StadiumApprovalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}