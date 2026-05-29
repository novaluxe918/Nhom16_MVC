namespace Nhom16_MVC.Models.DTOs
{
    public class LichDatDTO
    {
        public int MaDatSan { get; set; }
        public string TenKhachHang { get; set; }

        public string TenSanCon { get; set; }
        public string TenSanChinh { get; set; }

        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }

        public string TrangThai { get; set; }
        public decimal TongTien { get; set; }
    }
}