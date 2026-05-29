using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Repositories.Interfaces
{
    public interface IBangGiaRepository
    {
        Task<List<sanbongchitiet>> GetByChuSan(int chuSanId);

        Task<sanbongchitiet?> GetById(int id);

        Task Save();
    }
}