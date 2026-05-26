using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] 
    public class FinancialManagementController : ControllerBase
    {
        private readonly FinancialManagementService _financialService;

        public FinancialManagementController(FinancialManagementService financialService)
        {
            _financialService = financialService;
        }

        /// <summary>
        /// API lấy danh sách toàn bộ các yêu cầu rút tiền đang chờ duyệt
        /// URL: https://localhost:7295/api/FinancialManagement/pending-withdrawals
        /// </summary>
        [HttpGet("pending-withdrawals")]
        public async Task<IActionResult> GetPendingWithdrawals()
        {
            var result = await _financialService.GetPendingWithdrawalsAsync();
            return Ok(result);
        }

        /// <summary>
        /// API Duyệt hoặc Từ chối lệnh rút tiền
        /// </summary>
        [HttpPost("process-withdrawal")]
        public async Task<IActionResult> ProcessWithdrawal([FromBody] ProcessWithdrawalRequest request)
        {
            if (request == null || request.MaYeuCau <= 0)
            {
                return BadRequest(new FinancialManagementResponse { Success = false, Message = "Mã yêu cầu gửi lên không hợp lệ." });
            }

            if (request.TrangThaiMoi != "da_chuyen" && request.TrangThaiMoi != "tu_choi")
            {
                return BadRequest(new FinancialManagementResponse { Success = false, Message = "Trạng thái mới không hợp lệ." });
            }

            if (request.TrangThaiMoi == "tu_choi" && string.IsNullOrEmpty(request.LyDoTuChoi))
            {
                return BadRequest(new FinancialManagementResponse { Success = false, Message = "Vui lòng nhập lý do từ chối." });
            }

            var response = await _financialService.ProcessWithdrawalAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}