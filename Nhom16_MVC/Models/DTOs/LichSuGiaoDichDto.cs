namespace Nhom16_MVC.Models.DTOs
{
    public class LichSuGiaoDichDto
    {
        public string NgayGiaoDich { get; set; } = null!;
        public string LoaiHoatDong { get; set; } = null!;
        public long SoTien { get; set; }
        public string TrangThai { get; set; } = null!;
        public bool IsPositive { get; set; }

        // Cột này dùng để C# sắp xếp thời gian thật, không cần gửi xuống giao diện
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? ThoiGianThuc { get; set; }
    }
}