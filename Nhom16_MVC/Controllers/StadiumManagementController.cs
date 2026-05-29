using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] // 🔒 Chỉ Admin có Token hợp lệ mới truy cập được
    public class StadiumManagementController : ControllerBase
    {
        private readonly StadiumManagementService _stadiumService;

        public StadiumManagementController(StadiumManagementService stadiumService)
        {
            _stadiumService = stadiumService;
        }

        /// <summary>
        /// API lấy danh sách toàn bộ các sân bóng chưa duyệt kèm ảnh chi tiết
        /// </summary>
        [HttpGet("unapproved-stadiums")]
        public async Task<IActionResult> GetUnapprovedStadiums()
        {
            var result = await _stadiumService.GetUnapprovedStadiumsAsync();
            return Ok(result);
        }

        /// <summary>
        /// API phê duyệt hoặc từ chối yêu cầu tạo sân bóng (gửi mail tự động)
        /// </summary>
        [HttpPost("process-approval")]
        public async Task<IActionResult> ProcessApproval([FromBody] ApproveStadiumRequest request)
        {
            if (request == null || request.MaSanBong <= 0)
            {
                return BadRequest(new StadiumApprovalResponse
                {
                    Success = false,
                    Message = "Dữ liệu mã sân gửi lên không hợp lệ."
                });
            }

            var response = await _stadiumService.ProcessStadiumApprovalAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}