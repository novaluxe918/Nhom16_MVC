using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Repositories
{
    public interface ISanBongRepository
    {
        Task<List<sanbong>> LayTatCa();

        Task<sanbong?> LayTheoId(int id);

        Task Tao(sanbong san);

        void CapNhat(sanbong san);

        void Xoa(sanbong san);

        Task Save();
    }
}
