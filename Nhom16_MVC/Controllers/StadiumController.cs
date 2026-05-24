using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/sanbong")]
    public class StadiumController : ControllerBase
    {

        private readonly StadiumService _stadiumService;

        public StadiumController(StadiumService stadiumService)
        {
            _stadiumService = stadiumService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMySanBongs()
        {
            var chuSanId = await _stadiumService.GetFirstChuSanIdAsync();
            if (chuSanId == null)
            {
                return BadRequest(new
                {
                    message = "Không tìm thấy chủ sân"
                });
            }
            var result = await _stadiumService.GetMySanBongsAsync(chuSanId.Value);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSanBong([FromBody] CreateStadiumDto stadiumDto)
        {

            var chuSanId = await _stadiumService.GetFirstChuSanIdAsync();
            if (chuSanId == null)
            {
                return BadRequest(new
                {
                    message = "Không tìm thấy chủ sân"
                });
            }
            await _stadiumService.CreateSanBongAsync(chuSanId.Value, stadiumDto);
            return Ok(new
            {
                message = "Tạo sân bóng thành công"
            });

        }
    }
}
