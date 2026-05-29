using System;
using System.Text.Json.Serialization; // 👈 Bắt buộc phải có thư viện này để định danh JSON

namespace Nhom16_MVC.Models.DTOs
{
    public class AdminRatingViewDto
    {
        [JsonPropertyName("MaDanhGia")]
        public int MaDanhGia { get; set; }

        [JsonPropertyName("MaSanBong")]
        public int MaSanBong { get; set; }

        [JsonPropertyName("TenSanBong")] // 👈 Đảm bảo JSON bắn ra luôn là "TenSanBong" bất chấp cấu hình hệ thống
        public string TenSanBong { get; set; } = string.Empty;

        [JsonPropertyName("TenNguoiDung")]
        public string TenNguoiDung { get; set; } = string.Empty;

        [JsonPropertyName("SoSao")]
        public int SoSao { get; set; }

        [JsonPropertyName("NoiDung")]
        public string NoiDung { get; set; } = string.Empty;

        [JsonPropertyName("CreatedAt")]
        public DateTime? CreatedAt { get; set; }
    }

    public class DeleteRatingRequest
    {
        [JsonPropertyName("MaDanhGia")]
        public int MaDanhGia { get; set; }
    }

    public class RatingManagementResponse
    {
        [JsonPropertyName("Success")]
        public bool Success { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;
    }
}