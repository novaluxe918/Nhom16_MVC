using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Services;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/report")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/report/revenue/6
        [HttpGet("revenue/{chuSanId}")]
        public async Task<IActionResult> GetRevenueReport(
            int chuSanId)
        {
            try
            {
                var result =
                    await _reportService
                        .GetRevenueReport(chuSanId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        message = "Lỗi server",
                        error = ex.Message
                    }
                );
            }
        }
    }
}