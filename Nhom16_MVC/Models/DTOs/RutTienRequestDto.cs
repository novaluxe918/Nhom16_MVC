namespace Nhom16_MVC.Models.DTOs
{
    public class RutTienRequestDto
    {
        public long SoTien { get; set; }
        public string TenNganHang { get; set; } = null!;
        public string SoTaiKhoan { get; set; } = null!;
        public string? MoTa { get; set; }
    }
}
