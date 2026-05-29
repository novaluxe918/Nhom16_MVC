using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Repositories
{
    public interface ISanBongRepository
    {
      Task<List<sanbong>> GetAllAsync();

        Task<List<sanbong>> GetByChuSanAsync(int chusan);

        Task<sanbong?> GetByIdAsync(int id);

        Task AddAsync(sanbong san);

        Task UpdateAsync(sanbong san);

        Task DeleteAsync(sanbong san);

        Task SaveChangesAsync();
    }
}
