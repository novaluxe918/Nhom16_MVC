namespace Nhom16_MVC.Models.DTOs
{
    public class DatSanResponseDTO
    {
        public int maChiTietDatSan { get; set; }

        public int maDatSan { get; set; }

        public string tenNguoiDat { get; set; } = string.Empty;

        public string tenSan { get; set; } = string.Empty;

        public string tenSanChiTiet { get; set; } = string.Empty;

        public DateTime gioBatDau { get; set; }

        public DateTime gioKetThuc { get; set; }

        public string trangThaiDatSan { get; set; } = string.Empty;

        public bool coVanDe { get; set; }
    }
}