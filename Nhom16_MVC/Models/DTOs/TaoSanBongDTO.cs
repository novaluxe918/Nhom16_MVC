namespace Nhom16_MVC.Models.DTOs
{
    public class TaoSanBongDTO
    {
        public int ChuSan { get; set; }

        public string TenSan { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public string? DiaChi { get; set; }

        public string? Quan { get; set; }

        public string? Huyen { get; set; }

        public string? Xa { get; set; }

        public string? ThanhPho { get; set; }

        public string? HinhAnh { get; set; }

        public decimal? KinhDo { get; set; }

        public decimal? ViDo { get; set; }
    }
}
