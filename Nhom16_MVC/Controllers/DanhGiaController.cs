using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Services;
using System.Security.Claims;
using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhGiaController : Controller
    {
        private readonly DanhGiaService _danhGiaService;

        public DanhGiaController(DanhGiaService danhGiaService)
        {
            _danhGiaService = danhGiaService;
        }

        //api lấy ds bình luận 
        [HttpGet("san-con/{maSanChiTiet}")]
        public async Task<IActionResult> GetBinhLuan(int maSanChiTiet, [FromQuery] int page = 1)
        {
            if (page < 1) page = 1;
            var data = await _danhGiaService.GetBinhLuanSanConAsync(maSanChiTiet, page);
            return Ok(new { Success = true, Data = data });
        }

        //api gửi đánh giá mới 
        [HttpPost("gui-danh-gia")]
        public async Task<IActionResult> PostDanhGia([FromBody] DanhGiaRequestDto request)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized(new { Success = false, Message = "Chưa đăng nhập" });
            //int maNguoiDungDangNhap = int.Parse(userIdClaim);

            ////////////////////////////////////////////////////////

            int maNguoiDungDangNhap = 2;

            var result = await _danhGiaService.LuuDanhGiaCuaKhachAsync(maNguoiDungDangNhap, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
