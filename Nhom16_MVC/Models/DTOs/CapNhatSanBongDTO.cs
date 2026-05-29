namespace Nhom16_MVC.Models.DTOs
{
    public class CapNhatSanBongDTO
    {
      public string tensan { get; set; } = string.Empty;

        public string? mota { get; set; }

        public string? hinhanh { get; set; }

        public string? diachi { get; set; }

        public string? quan { get; set; }

        public string? huyen { get; set; }

        public string? xa { get; set; }

        public string? thanhpho { get; set; }

        public decimal? kinhdo { get; set; }

        public decimal? vido { get; set; }

        public TimeOnly giomocua { get; set; }

        public TimeOnly giodongcua { get; set; }
    }
}
