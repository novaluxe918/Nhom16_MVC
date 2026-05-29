using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/sanbong")]
    public class SanBongController : ControllerBase
    {

        private readonly SearchService _searchService;
        private readonly AvailableFieldService _availableFieldService;
        private readonly SanBongService _sanBongService;

        private readonly ISanBongService _service;

        public SanBongController(SearchService searchService, AvailableFieldService availableFieldService, SanBongService sanBongService, ISanBongService service)
        {
            _searchService = searchService;
            _availableFieldService = availableFieldService;
            _sanBongService = sanBongService;
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

        [HttpGet]
        public async Task<IActionResult> LayTatCaSan()
        {
            var data = await _service.LayTatCaSan();

            return Ok(data);
        }

        [HttpGet("chusan/{id}")]
        public async Task<IActionResult> LaySanTheoChuSan(int id)
        {
            var data = await _service.LaySanTheoChuSan(id);

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> LaySanTheoId(int id)
        {
            var data = await _service.LaySanTheoId(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> TaoSan(
    [FromForm] TaoSanBongDTO dto,
    [FromQuery] int chusan)
        {
            var result = await _service.TaoSan(dto, chusan);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CapNhatSan(
            int id,
            [FromBody] CapNhatSanBongDTO dto)
        {
            var result = await _service.CapNhatSan(id, dto);

            if (!result)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> XoaSan(int id)
        {
            var result = await _service.XoaSan(id);

            if (!result)
                return NotFound();

            return Ok(result);
        }
    }
}