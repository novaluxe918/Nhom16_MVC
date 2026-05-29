namespace Nhom16_MVC.Models.DTOs
{
    public class CreateSanConDTO
    {
        public int masanbong { get; set; }

        public int maloaisan { get; set; }

        public string tensanchitiet { get; set; } = string.Empty;

        public long giathuebuoisang { get; set; }

        public long giathuebuoitoi { get; set; }
    }
}