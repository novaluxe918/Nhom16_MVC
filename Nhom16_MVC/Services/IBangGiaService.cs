using Nhom16_MVC.Models.DTOs.BangGia;

namespace Nhom16_MVC.Services.Interfaces
{
    public interface IBangGiaService
    {
        Task<List<SanChiTietGiaDTO>> GetByChuSan(int chuSanId);

        Task<bool> CapNhatGia(int maSanChiTiet, CapNhatGiaDTO dto);
    }
}