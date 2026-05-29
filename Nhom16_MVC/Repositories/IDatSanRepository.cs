using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Repositories
{
    public interface IDatSanRepository
    {
        Task<List<DatSanResponseDTO>> GetLichDatByChuSan(
            int chusan,
            FilterDatSanDTO filter);
    }
}