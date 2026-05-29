using System.Collections.Generic;

namespace Nhom16_MVC.Models.DTOs
{
    public class StadiumApprovalViewDto
    {
        public int MaSanBong { get; set; }
        public string TenSan { get; set; } = string.Empty;
        public string ChuSan { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string HinhAnhDaiDien { get; set; } = string.Empty;
        public List<string> DanhSachHinhAnhChiTiet { get; set; } = new List<string>();
        public bool DaDuyet { get; set; }
    }

    public class ApproveStadiumRequest
    {
        public int MaSanBong { get; set; } // Sửa từ long sang int cho khớp Database
        public bool IsApproved { get; set; }
        public string LyDoTuChoi { get; set; } = string.Empty;
    }

    public class StadiumApprovalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}