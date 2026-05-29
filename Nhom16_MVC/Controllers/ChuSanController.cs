using Microsoft.AspNetCore.Mvc;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services.Interfaces;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/chu-san")]
    public class ChuSanController : ControllerBase
    {
        private readonly IDatSanService _datSanService;

        public ChuSanController(IDatSanService datSanService)
        {
            _datSanService = datSanService;
        }

        [HttpGet("lich-dat-san")]
        public IActionResult GetLichDat([FromQuery] FilterLichDatDto filter)
        {
            var data = _datSanService.GetLichDat(filter);
            return Ok(data);
        }
    }
}