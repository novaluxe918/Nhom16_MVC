using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Services
{
    public interface IDatSanService
    {
        Task<List<DatSanResponseDTO>> GetLichDat(
            int chusan,
            FilterDatSanDTO filter);
    }
}