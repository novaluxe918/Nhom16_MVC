using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Security.Claims;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiaoDichController : Controller
    {
        private readonly GiaoDichService _giaoDichService;
        public GiaoDichController(GiaoDichService giaoDichService) { _giaoDichService = giaoDichService; }

        //api1 rút tiền
        [HttpPost("rut-tien")]
        public async Task<IActionResult> RutTien([FromBody] RutTienRequestDto reqest)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //kiểm tra thẻ id
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Success = false, Message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn." });
            }

            int maNguoiDungDangNhap = int.Parse(userIdClaim);

            var result = await _giaoDichService.TaoYeuCauRutTienAsync(maNguoiDungDangNhap, reqest);
            if (!result.Success) return BadRequest(result);
            return Ok(result);

        }

        //api nạp tiềm 
        [HttpPost("nap-tien")]
        public async Task<IActionResult> TaoUrlNapTien([FromBody] NapTienRequestDto request)
        {
            
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized(new { Success = false, Message = "Chưa đăng nhập." });

            int maNguoiDungThat = int.Parse(userIdClaim);

            if (request.SoTien < 10000) return BadRequest(new { Success = false, Message = "Số tiền nạp tối thiểu 10.000đ" });

            string url = await _giaoDichService.TaoUrlNapTienVnPayAsync(maNguoiDungThat, request.SoTien);

            return Ok(new { Success = true, Url = url });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            string ketQua = await _giaoDichService.XuLyVnPayReturnAsync(Request.Query);

           
            // Tạm thời để test API:
            return Ok(new { Message = ketQua });
        }

    }
}
