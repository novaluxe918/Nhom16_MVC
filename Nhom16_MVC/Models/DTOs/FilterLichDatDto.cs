using System;

namespace Nhom16_MVC.Models.DTOs
{
    public class FilterLichDatDto
    {
        public int ChuSanId { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public string? TrangThai { get; set; }

        public int? MaSanChiTiet { get; set; }
    }
}