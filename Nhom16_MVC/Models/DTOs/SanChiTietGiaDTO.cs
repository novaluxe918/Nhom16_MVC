namespace Nhom16_MVC.Models.DTOs.BangGia
{
    public class SanChiTietGiaDTO
    {
        public int MaSanChiTiet { get; set; }

        public string TenSanChiTiet { get; set; } = string.Empty;

        public string? LoaiSan { get; set; }

        public long GiaBuoiSang { get; set; }

        public long GiaBuoiToi { get; set; }
    }
}