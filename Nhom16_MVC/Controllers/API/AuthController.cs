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
                return Redirect("/Home/VerificationSuccess");
            }

            return Redirect($"/Home/VerificationFailed?message={result.Message}");
        }

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!"
                });
            }

            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ForgotPasswordResponse { Success = false, Message = "Email nhập vào không đúng định dạng." });
            }

            var response = await _authService.ForgotPasswordAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResetPasswordResponse { Success = false, Message = "Dữ liệu nhập vào không hợp lệ." });
            }

            var response = await _authService.ResetPasswordAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

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
                Timestamp = System.DateTime.Now
            });
        }
    }
    public class ResendVerificationRequest
    {
        public string Email { get; set; }
    }
}