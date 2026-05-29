using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Services.Interfaces
{
    public interface IDatSanService
    {
        List<LichDatSanDto> GetLichDat(FilterLichDatDto filter);
    }
}