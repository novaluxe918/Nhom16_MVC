using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] 
    public class UserManagementController : ControllerBase
    {
        private readonly UserManagementService _userManagementService;

        public UserManagementController(UserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Lấy danh sách toàn bộ người dùng (Có thể lọc theo query string: ?role=chuSan hoặc ?role=nguoiThue)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? role)
        {
            var response = await _userManagementService.GetAllUsersAsync(role);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Khóa hoặc mở khóa tài khoản dựa vào MaNguoiDung và trạng thái IsLocked truyền lên
        /// </summary>
        [HttpPut("toggle-lock")]
        public async Task<IActionResult> ToggleLockUser([FromBody] ToggleLockDto request)
        {
            if (request == null || request.MaNguoiDung <= 0)
            {
                return BadRequest(new UserManagementResponse
                {
                    Success = false,
                    Message = "Dữ liệu gửi lên không hợp lệ."
                });
            }

            // Gọi hàm xử lý cập nhật trạng thái chuỗi 'trangthai' trong Service mới
            var response = await _userManagementService.ToggleUserLockAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}