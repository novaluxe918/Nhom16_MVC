using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/datsan")]
    public class DatSanController : ControllerBase
    {
        private readonly IDatSanService _service;

        public DatSanController(IDatSanService service)
        {
            _service = service;
        }

        [HttpGet("chusan/{chusan}")]
        public async Task<IActionResult> GetLichDat(
            int chusan,
            [FromQuery] FilterDatSanDTO filter)
        {
            var data = await _service.GetLichDat(chusan, filter);

            return Ok(data);
        }
    }
}