using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Security.Claims;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanBongChiTietController : ControllerBase
    {
        private readonly SanBongChiTietService _sanBongChiTietService;
        private readonly BookingService _bookingService;

        private readonly ISanBongChiTietService _service;

        public SanBongChiTietController(SanBongChiTietService sanBongChiTietService, BookingService bookingService,ISanBongChiTietService service)
        {
            _sanBongChiTietService = sanBongChiTietService;
            _bookingService = bookingService;
            _service = service;
        }
        
         [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("san/{id}")]
        public async Task<IActionResult> GetBySan(int id)
        {
            return Ok(await _service.GetBySanBong(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSanConDTO dto)
        {
            return Ok(await _service.Create(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSanConDTO dto)
        {
            var result = await _service.Update(id, dto);

            if (!result) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);

            if (!result) return NotFound();

            return Ok(result);
        }
        //api lấy tt cơ bản của sân con 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetThongTinSanCon(int id)
        {
            var data = await _sanBongChiTietService.GetThongTinSanConAsync(id);
            if (data == null)
            {
                return NotFound(new { Success = false, Message = "Sân chi tiết không tồn tại hoặc sân mẹ chưa được duyệt." });
            }
            return Ok(new { Success = true, Data = data });
        }

        //api2 lấy lịch trống sân con theo ngày củ thể

        [HttpGet("{id}/lich-trong")]
        public async Task<IActionResult> GetLichTrong(int id, [FromQuery] string ngay)
        {
            // Kiểm tra định dạng ngày mà Frontend gửi lên có chuẩn không
            if (!DateOnly.TryParseExact(ngay, "yyyy-MM-dd", out DateOnly ngayChon))
            {
                return BadRequest(new { Success = false, Message = "Định dạng ngày không hợp lệ. Yêu cầu chuẩn: yyyy-MM-dd" });
            }

            var data = await _sanBongChiTietService.GetLichTrongTheoNgayAsync(id, ngayChon);

            if (data == null)
            {
                return NotFound(new { Success = false, Message = "Lỗi truy xuất dữ liệu lịch sân." });
            }

            return Ok(new { Success = true, Data = data });
        }

        //api3 : tiếp nhận yêu cầu đặt sân
        [HttpPost("{id}/dat-lich")]
        public async Task<IActionResult> DatLichSanCon(int id, [FromBody] DatSanYeuCauDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //kiểm tra thẻ id
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Success = false, Message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn." });
            }

            int maNguoiDungDangNhap = int.Parse(userIdClaim);

            // Kiểm tra đầu vào 
            if (request == null || request.DanhSachSlotDat == null || request.DanhSachSlotDat.Count == 0)
            {
                return BadRequest(new searchSuggestResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một slot khung giờ để đặt sân."
                });
            }

            // Gọi Service xử lý giao dịch tiền tệ
            var result = await _bookingService.XuLyDatSanTransactionAsync(maNguoiDungDangNhap, id, request);

            if (!result.Success)
            {
                
                if (result.Message.Contains("người khác đặt mất") || result.Message.Contains("Số dư"))
                {
                    return BadRequest(result);
                }
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        //api4 hủy sân
        [HttpPost("huy-lich/{maChiTiet}")]
        public async Task<IActionResult> HuyLichDat(int maChiTiet)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //kiểm tra thẻ id
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Success = false, Message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn." });
            }

            int maNguoiDungDangNhap = int.Parse(userIdClaim);


            var result = await _bookingService.HuyDatSanVaHoanTienAsync(maNguoiDungDangNhap, maChiTiet);

            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
