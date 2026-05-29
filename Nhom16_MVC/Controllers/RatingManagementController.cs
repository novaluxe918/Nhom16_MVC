using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] // 🔒 Bảo mật bằng Token Admin
    public class RatingManagementController : ControllerBase
    {
        private readonly RatingManagementService _ratingService;

        public RatingManagementController(RatingManagementService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>
        /// API lấy danh sách toàn bộ các đánh giá trên hệ thống để Admin kiểm duyệt
        /// </summary>
        [HttpGet("all-ratings")]
        public async Task<IActionResult> GetAllRatings()
        {
            var result = await _ratingService.GetAllRatingsAsync();
            return Ok(result);
        }

        /// <summary>
        /// API gỡ bỏ đánh giá mang tính chất spam, ảo, xúc phạm
        /// </summary>
        [HttpDelete("delete-rating")]
        public async Task<IActionResult> DeleteRating([FromBody] DeleteRatingRequest request)
        {
            if (request == null || request.MaDanhGia <= 0)
            {
                return BadRequest(new RatingManagementResponse
                {
                    Success = false,
                    Message = "Mã định danh đánh giá gửi lên không đúng định dạng."
                });
            }

            var response = await _ratingService.DeleteRatingAsync(request.MaDanhGia);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}