namespace Nhom16_MVC.Models.DTOs
{
    
        public class searchSuggestRequest
        {
            [System.ComponentModel.DataAnnotations.MaxLength(255)]    
            //từ khóa tìm kiếm bắt buộc 
            public string Query { get; set; } = string.Empty;
        }

        public class searchSuggestResponse
        {
            public bool Success { get; set; } = true;
            //danh sách kết quả gợi ý tìm kiếm
            public List<string> Suggestions { get; set; } = new List<string>();
            public string Message { get; set; } = "OK";
        }
    }

