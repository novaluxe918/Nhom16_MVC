using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Services
{
    public interface ISanBongChiTietService
    {
        Task<List<sanbongchitiet>> GetAll();

        Task<List<sanbongchitiet>> GetBySanBong(int masanbong);

        Task<bool> Create(CreateSanConDTO dto);

        Task<bool> Update(int id, UpdateSanConDTO dto);

        Task<bool> Delete(int id);
    }
}