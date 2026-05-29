using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Repositories;

namespace Nhom16_MVC.Services
{
    public class DatSanService : IDatSanService
    {
        private readonly IDatSanRepository _repo;

        public DatSanService(IDatSanRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<DatSanResponseDTO>> GetLichDat(
            int chusan,
            FilterDatSanDTO filter)
        {
            return await _repo.GetLichDatByChuSan(chusan, filter);
        }
    }
}