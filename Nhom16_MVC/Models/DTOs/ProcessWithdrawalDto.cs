namespace Nhom16_MVC.Models.DTOs
{
    public class ProcessWithdrawalDto
    {
        public int MaYeuCau { get; set; }
        public string TrangThaiMoi { get; set; } // 'da_chuyen' hoặc 'tu_choi'
        public string? LyDoTuChoi { get; set; }
    }

    public class WithdrawalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}