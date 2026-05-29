namespace Nhom16_MVC.Models.DTOs
{
    public class DanhGiaRequestDto
    {
        public int MaSanChiTiet { get; set; }
        public short DiemSo { get; set; } // Số sao từ 1 đến 5
        public string? BinhLuan { get; set; } // Nội dung nhận xét văn bản
    }

    public class BinhLuanHienThiDto
    {
        public int MaDanhGia { get; set; }
        public string TenNguoiDung { get; set; } = null!;
        public string? AvatarNguoiDung { get; set; }
        public short DiemSo { get; set; }
        public string? BinhLuan { get; set; }
        public string ThoiGian { get; set; } = null!; // Định dạng chuỗi: "dd/MM/yyyy HH:mm"
    }

    public class DanhSachBinhLuanPhanTrangDto
    {
        public List<BinhLuanHienThiDto> ListBinhLuan { get; set; } = new();
        public int TrangHienTai { get; set; }
        public int TongSoTrang { get; set; }
        public int TongSoBinhLuan { get; set; }
    }
}