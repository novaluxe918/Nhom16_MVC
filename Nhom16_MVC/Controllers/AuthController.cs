using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthDTOs model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState });

            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
                return StatusCode(result.ErrorDetail != null ? 500 : 400, new { success = false, message = result.Message, error = result.ErrorDetail });

            return Ok(new { success = true, message = result.Message, userId = result.Data, emailSent = result.EmailSent });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ!" });

            var result = await _authService.VerifyEmailAsync(model);
            if (!result.Success)
                return StatusCode(result.ErrorDetail != null ? 500 : 400, new { success = false, message = result.Message, error = result.ErrorDetail });

            return Ok(new { success = true, message = result.Message, userId = result.Data });
        }

        [HttpPost("resend-email-otp")]
        public async Task<IActionResult> ResendEmailOtp([FromBody] ForgotPasswordDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Email không hợp lệ!" });

            var result = await _authService.ResendEmailOtpAsync(model);
            if (!result.Success)
                return StatusCode(result.ErrorDetail != null ? 500 : 400, new { success = false, message = result.Message, error = result.ErrorDetail });

            return Ok(new { success = true, message = result.Message, emailSent = result.EmailSent });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Email và mật khẩu không hợp lệ!" });

            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                if (result.RequiresEmailVerification)
                {
                    return BadRequest(new { success = false, message = result.Message, requiresEmailVerification = true, userId = result.Data?.UserId });
                }
                return Unauthorized(new { success = false, message = result.Message, error = result.ErrorDetail });
            }

            // 🌟 TRẢ THÊM TRƯỜNG TOKEN RA CHO POSTMAN/FRONTEND
            return Ok(new
            {
                success = true,
                message = result.Message,
                userId = result.Data?.UserId,
                email = result.Data?.Email,
                hoTen = result.Data?.HoTen,
                vaiTro = result.Data?.VaiTro,
                token = result.Data?.Token // Đã xuất trường token ở đây
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Email không hợp lệ!" });

            var result = await _authService.ForgotPasswordAsync(model);
            if (!result.Success)
                return StatusCode(result.ErrorDetail != null ? 500 : 400, new { success = false, message = result.Message, error = result.ErrorDetail });

            return Ok(new { success = true, message = result.Message, userId = result.Data, emailSent = result.EmailSent });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ!" });

            var result = await _authService.ResetPasswordAsync(model);
            if (!result.Success)
                return StatusCode(result.ErrorDetail != null ? 500 : 400, new { success = false, message = result.Message, error = result.ErrorDetail });

            return Ok(new { success = true, message = result.Message });
        }
    }
}