using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Services
{
    public interface ISanBongService
    {
      Task<List<sanbong>> LayTatCaSan();

        Task<List<sanbong>> LaySanTheoChuSan(int chusan);

        Task<sanbong?> LaySanTheoId(int id);

        Task<bool> TaoSan(TaoSanBongDTO dto, int chusan);

        Task<bool> CapNhatSan(int id, CapNhatSanBongDTO dto);

        Task<bool> XoaSan(int id);
    }
}
