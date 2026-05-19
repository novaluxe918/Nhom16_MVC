using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// API Đăng ký tài khoản mới
        /// POST: api/auth/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!",
                    Data = ModelState
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// API Xác thực email qua token
        /// GET: api/auth/verify-email?token=xxx
        /// </summary>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Token không được để trống!"
                });
            }

            var result = await _authService.VerifyEmailAsync(token);

            if (result.Success)
            {
                // Redirect về trang thông báo thành công (hoặc trang đăng nhập)
                return Redirect("/Home/VerificationSuccess");
            }

            // Redirect về trang thông báo lỗi
            return Redirect($"/Home/VerificationFailed?message={result.Message}");
        }

        /// <summary>
        /// API Gửi lại email xác thực
        /// POST: api/auth/resend-verification
        /// </summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Email không được để trống!"
                });
            }

            var result = await _authService.ResendVerificationEmailAsync(request.Email);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// API Test kết nối database
        /// GET: api/auth/test-db
        /// </summary>
        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabase([FromServices] DatabaseService dbService)
        {
            var isConnected = await dbService.TestConnectionAsync();

            return Ok(new
            {
                Success = isConnected,
                Message = isConnected
                    ? "✅ Kết nối PostgreSQL thành công!"
                    : "❌ Không thể kết nối đến PostgreSQL!",
                Timestamp = DateTime.Now
            });
        }
    }

    // DTO cho resend verification
    public class ResendVerificationRequest
    {
        public string Email { get; set; }
    }
}