using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs.BangGia;
using Nhom16_MVC.Services.Interfaces;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BangGiaController : ControllerBase
    {
        private readonly IBangGiaService _service;

        public BangGiaController(IBangGiaService service)
        {
            _service = service;
        }

        // Lấy bảng giá theo chủ sân
        [HttpGet("chu-san/{chuSanId}")]
        public async Task<IActionResult> GetByChuSan(int chuSanId)
        {
            var data = await _service.GetByChuSan(chuSanId);
            return Ok(data);
        }

        // Cập nhật giá sân
        [HttpPut("{maSanChiTiet}")]
        public async Task<IActionResult> CapNhat(int maSanChiTiet, CapNhatGiaDTO dto)
        {
            var result = await _service.CapNhatGia(maSanChiTiet, dto);

            if (!result)
                return NotFound("Không tìm thấy sân");

            return Ok("Cập nhật giá thành công");
        }
    }
}