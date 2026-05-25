using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Services
{
   public interface ISanBongService
    {
        Task<List<SanBongDTO>> LayTatCa();

        Task<SanBongDTO?> LayTheoId(int id);

        Task<bool> TaoSan(CreateSanBongDto dto);

        Task<bool> CapNhatSan(int id, UpdateSanBongDto dto);

        Task<bool> XoaSan(int id);
    }
}