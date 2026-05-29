using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Repositories
{
    public interface ISanBongChiTietRepository
    {
        Task<List<sanbongchitiet>> GetAllAsync();

        Task<List<sanbongchitiet>> GetBySanBongIdAsync(int masanbong);

        Task<sanbongchitiet?> GetByIdAsync(int id);

        Task AddAsync(sanbongchitiet entity);

        Task DeleteAsync(sanbongchitiet entity);

        Task SaveAsync();
    }
}