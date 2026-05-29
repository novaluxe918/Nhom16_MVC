namespace Nhom16_MVC.Models.DTOs
{
    public class ThongTinViDto
    {
        public string HoTen { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public long SoDu { get; set; }
        public string TenNganHang { get; set; } = "";
        public string SoTaiKhoan { get; set; } = "";
        public List<LichSuGiaoDichDto> LichSu { get; set; } = new List<LichSuGiaoDichDto>();
    }
}