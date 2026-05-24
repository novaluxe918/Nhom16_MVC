namespace Nhom16_MVC.Models.DTOs
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}