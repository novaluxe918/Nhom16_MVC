using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanBongController : ControllerBase
    {

        private readonly SearchService _searchService;
        private readonly AvailableFieldService _availableFieldService;


        public SanBongController(SearchService searchService, AvailableFieldService availableFieldService)
        {
            _searchService = searchService;
            _availableFieldService = availableFieldService;
        }

        [HttpGet("goi-y")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {

                return BadRequest(new searchSuggestResponse
                {
                    Success = false,
                    Suggestions = new List<string>(),
                    Message = "Từ khóa tìm kiếm không được rỗng"
                });
            }


            var suggestions = await _searchService.GetSuggestionsAsync(q);

            return Ok(new searchSuggestResponse
            {
                Success = true,
                Suggestions = suggestions,
                Message = suggestions.Count > 0 ? "Tìm kiếm thành công" : "Không tìm thấy sân bóng"
            });
        }

        /// Tìm kiếm sân trống theo khung giờ

        [HttpPost("tim-kiem-san-trong")]
        public async Task<IActionResult> SearchAvailableFields([FromBody] SearchAvailableFieldsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new SearchAvailableFieldsResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            var result = await _availableFieldService.SearchAvailableFieldsAsync(request);
            return Ok(result);
        }
    }
}
