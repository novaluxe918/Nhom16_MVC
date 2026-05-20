using System.Collections.Generic;

namespace Nhom16_MVC.Models.DTOs
{
    // DTO hiển thị thông tin sân kèm TẤT CẢ hình ảnh để Admin kiểm duyệt
    public class StadiumApprovalViewDto
    {
        public int MaSanBong { get; set; }
        public string TenSan { get; set; }
        public string ChuSan { get; set; }
        public string DiaChi { get; set; }
        public string MoTa { get; set; }
        public string HinhAnhDaiDien { get; set; } // Ảnh chính trong bảng SANBONG
        public List<string> DanhSachHinhAnhChiTiet { get; set; } = new List<string>(); // Gom tất cả ảnh từ bảng MEDIA_SANBONG
        public bool DaDuyet { get; set; }
    }

    public class ApproveStadiumRequest
    {
        public int MaSanBong { get; set; }
    }

    public class StadiumApprovalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}