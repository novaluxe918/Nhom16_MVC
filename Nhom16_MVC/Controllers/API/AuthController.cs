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

        [HttpGet("admin/unapproved-stadiums")]
        public async Task<IActionResult> GetUnapprovedStadiums()
        {
            var result = await _authService.GetUnapprovedStadiumsAsync();
            return Ok(result);
        }

        [HttpPost("admin/process-approval")]
        public async Task<IActionResult> ProcessStadiumApproval([FromBody] ApproveStadiumRequest request)
        {
            if (request == null || request.MaSanBong <= 0)
            {
                return BadRequest(new StadiumApprovalResponse { Success = false, Message = "Mã sân bóng không hợp lệ." });
            }

            var response = await _authService.ProcessStadiumApprovalAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("admin/pending-withdrawals")]
        public async Task<IActionResult> GetPendingWithdrawals()
        {
            var result = await _authService.GetPendingWithdrawalsAsync();
            return Ok(result);
        }

        [HttpPost("admin/process-withdrawal")]
        public async Task<IActionResult> ProcessWithdrawal([FromBody] ProcessWithdrawalDto request)
        {
            if (request == null || request.MaYeuCau <= 0)
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Mã yêu cầu không hợp lệ." });
            }

            if (request.TrangThaiMoi != "da_chuyen" && request.TrangThaiMoi != "tu_choi")
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Trạng thái mới không hợp lệ. Chỉ nhận 'da_chuyen' hoặc 'tu_choi'." });
            }

            if (request.TrangThaiMoi == "tu_choi" && string.IsNullOrEmpty(request.LyDoTuChoi))
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Vui lòng nhập lý do từ chối yêu cầu rút tiền." });
            }

            var response = await _authService.ProcessWithdrawalAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("admin/update-user-status")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusDto request)
        {
            if (request == null || request.MaNguoiDung <= 0)
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Thông tin tài khoản không hợp lệ." });
            }

            if (request.TrangThaiMoi != "hoat_dong" && request.TrangThaiMoi != "bi_khoa")
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Trạng thái mới không hợp lệ! Chỉ nhận 'hoat_dong' hoặc 'bi_khoa'." });
            }

            var response = await _authService.UpdateUserStatusAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("admin/all-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int currentAdminId)
        {
            // currentAdminId này khi làm thực tế frontend sẽ truyền ID của thằng admin đang log vào qua query
            if (currentAdminId <= 0)
            {
                return BadRequest(new { Success = false, Message = "Mã Admin hiện tại không hợp lệ." });
            }

            var result = await _authService.GetAllUsersExceptAdminAsync(currentAdminId);
            return Ok(result);
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
        [HttpGet("admin/bookings")]
        public async Task<IActionResult> AdminGetAllBookings()
        {
            var result = await _authService.AdminGetAllBookingsAsync();
            return Ok(result);
        }

        [HttpPost("admin/resolve-booking")]
        public async Task<IActionResult> AdminResolveBooking([FromBody] ResolveBookingIssueDto request)
        {
            if (request == null || request.MaChiTietDatSan <= 0)
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Dữ liệu gửi lên không hợp lệ." });
            }

            var response = await _authService.AdminResolveBookingIssueAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("admin/ratings")]
        public async Task<IActionResult> AdminGetAllRatings()
        {
            var result = await _authService.AdminGetAllRatingsAsync();
            return Ok(result);
        }

        [HttpDelete("admin/delete-rating")]
        public async Task<IActionResult> AdminDeleteRating([FromBody] DeleteRatingDto request)
        {
            if (request == null || request.MaDanhGia <= 0)
            {
                return BadRequest(new WithdrawalResponse { Success = false, Message = "Mã đánh giá không hợp lệ." });
            }

            var response = await _authService.AdminDeleteRatingAsync(request.MaDanhGia);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
    public class ResendVerificationRequest
    {
        public string Email { get; set; }
    }
}