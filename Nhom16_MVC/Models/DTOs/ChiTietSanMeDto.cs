namespace Nhom16_MVC.Models.DTOs
{
    public class ChiTietSanMeDto
    {
        public int MaSanBong { get; set; }
        public string TenSan { get; set; } = null!;
        public string? MoTa { get; set; }
        public string? DiaChi { get; set; }
        public string? Quan {  get; set; }
        public string? ThanhPhos { get; set; }
        public decimal? KinhDo {  get; set; }
        public decimal? ViDo { get; set; }
        public string? GioHoatDong { get; set; }

        public List<string> AlbumMedia { get; set; } = new();
        public List<SanConTrongSanMeDto> DanhSachSanCon { get; set;} = new();

    }

    public class SanConTrongSanMeDto
    {
        public int MaSanChiTiet { get; set;  }
        public string TenSanChiTiet { get; set; } = null!;
        public string? LoaiSan { get; set; }
        public long GiaThueBuoiSang { get; set; }
        public long GiaThueBuoiToi { get; set; }
        public string? AnhDaiDien { get; set;  }

    }
}
