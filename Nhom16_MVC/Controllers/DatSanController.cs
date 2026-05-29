using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Services;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatSanController : ControllerBase
    {
        private readonly BookingService _bookingService;

        // Tiêm Service vào Controller qua Constructor
        public DatSanController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("lich-su/{userId}")]
        public async Task<IActionResult> GetLichSuCuaToi(int userId)
        {
            

            var result = await _bookingService.GetLichSuDatSanAsync(userId);

            // Trả kết quả về cho React
            if (result == null || !result.Any())
            {
                return Ok(new { success = true, data = result, message = "Chưa có lịch sử đặt sân." });
            }

            return Ok(new { success = true, data = result });
        }
    }

}
