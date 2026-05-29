namespace Nhom16_MVC.Models.DTOs
{
    public class DatSanYeuCauDto
    {
        public List<SlotYeuCauDto> DanhSachSlotDat { get; set; } = new();
    }

    public class SlotYeuCauDto
    {
        public string Ngay { get; set; } = null!;      // Ví dụ: "2026-05-26"
        public string GioBatDau { get; set; } = null!; // Ví dụ: "18:00"
    }
}
