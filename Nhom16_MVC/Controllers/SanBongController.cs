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
        private readonly SanBongService _sanBongService;


        public SanBongController(SearchService searchService, AvailableFieldService availableFieldService, SanBongService sanBongService)
        {
            _searchService = searchService;
            _availableFieldService = availableFieldService;
            _sanBongService = sanBongService;
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


        [HttpGet("danh-sach-loai-san")]
        public async Task<IActionResult> GetLoaiSan()
        {
            var result = await _searchService.GetDanhSachLoaiSanAsync();
            return Ok(new { Success = true, Data = result });
        }

        [HttpGet("danh-sach-quan")]
        public async Task<IActionResult> GetQuan()
        {
            var result = await _searchService.GetDanhSachQuanAsync();
            return Ok(new { Success = true, Data = result });
        }

        //api sân mẹ 
        [HttpGet("chi-tiet-san-me/{id}")]
        public async Task<IActionResult> GetChiTietSanMe(int id)
        {
            var result = await _sanBongService.GetChiTietSanMeAsync(id);

            if (result == null)
            {
                return NotFound(new { Success = false, Message = "Không tìm thấy dữ liệu sân bóng hoặc sân chưa được phê duyệt." });
            }

            return Ok(new { Success = true, Data = result });
        }

        [HttpGet("danh-sach-san-me")]
        public async Task<IActionResult> GetDanhSachSanMe([FromQuery] string? tenSan)
        {
            var result = await _sanBongService.GetDanhSachSanMeAsync(tenSan);
            return Ok(new { Success = true, Data = result });
        }
    }
}