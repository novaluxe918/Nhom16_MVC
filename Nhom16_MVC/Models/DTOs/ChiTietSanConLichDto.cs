namespace Nhom16_MVC.Models.DTOs
{
    public class ChiTietSanConLichDto
    {
        public int MaSanChiTiet { get; set; }
        public string TenSanChiTiet { get; set; } = null!;
        public string? LoaiSan { get; set; }
        public long GiaBuoiSang { get; set; }
        public long GiaBuoiToi { get; set; }

        // Thông tin sân mẹ kèm theo
        public int MaSanBongMenge { get; set; }
        public string TenSanMenge { get; set; } = null!;
        public string? DiaChiMenge { get; set; }
        public decimal DiemTrungBinh { get; set; }
        public int TongSoBinhLuan { get; set; }

        public string GioHoatDong { get; set; } = null!;


        public List<string> AlbumMediaSanCon { get; set; } = new();
        
    }

    public class LichTrongTheoNgayDto
    {
        public string Ngay { get; set; } = null!; // Ví dụ: "26/05/2026"
        public List<SlotGioTrangThaiDto> Slots { get; set; } = new();
    }

    public class SlotGioTrangThaiDto
    {
        public string GioBatDau { get; set; } = null!;  // "17:00"
        public string GioKetThuc { get; set; } = null!; // "18:00"
        public bool ConTrong { get; set; }              // true = Xanh, false = Xám (Đã đặt)
    }

}
