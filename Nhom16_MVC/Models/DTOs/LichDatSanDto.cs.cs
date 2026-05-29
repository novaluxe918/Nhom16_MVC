using System;

namespace Nhom16_MVC.Models.DTOs
{
    public class LichDatSanDto
    {
        public int MaDatSan { get; set; }

        public string TenNguoiDat { get; set; } = null!;

        public string TenSanCon { get; set; } = null!;

        public DateTime NgayDat { get; set; }

        public DateTime GioBatDau { get; set; }

        public DateTime GioKetThuc { get; set; }

        public string TrangThai { get; set; } = null!;
    }
}