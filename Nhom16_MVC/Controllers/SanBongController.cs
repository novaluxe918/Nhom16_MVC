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
        private readonly ISanBongService _service;

        public SanBongController(SearchService searchService, AvailableFieldService availableFieldService, ISanBongService service)
        {
            _searchService = searchService;
            _availableFieldService = availableFieldService;
            _service = service;
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


        [HttpGet]
        public async Task<IActionResult> LayTatCa()
        {
            var data = await _service.LayTatCa();

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> LayTheoId(int id)
        {
            var data = await _service.LayTheoId(id);

            if (data == null)
                return NotFound("Không tìm thấy sân bóng");

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> TaoSan([FromBody] TaoSanBongDTO dto)
        {
            var result = await _service.TaoSan(dto);

            if (!result)
                return BadRequest("Tạo sân thất bại");

            return Ok("Tạo sân thành công");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CapNhatSan(int id, [FromBody] CapNhatSanBongDTO dto)
        {
            var result = await _service.CapNhatSan(id, dto);

            if (!result)
                return NotFound("Không tìm thấy sân bóng");

            return Ok("Cập nhật thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> XoaSan(int id)
        {
            var result = await _service.XoaSan(id);

            if (!result)
                return NotFound("Không tìm thấy sân bóng");

            return Ok("Xóa thành công");
        }
    }
}