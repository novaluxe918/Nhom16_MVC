using Nhom16_MVC.Models.DTOs.BangGia;
using Nhom16_MVC.Repositories.Interfaces;
using Nhom16_MVC.Services.Interfaces;

namespace Nhom16_MVC.Services
{
    public class BangGiaService : IBangGiaService
    {
        private readonly IBangGiaRepository _repo;

        public BangGiaService(IBangGiaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SanChiTietGiaDTO>> GetByChuSan(int chuSanId)
        {
            var data = await _repo.GetByChuSan(chuSanId);

            return data.Select(x => new SanChiTietGiaDTO
            {
                MaSanChiTiet = x.masanchitiet,
                TenSanChiTiet = x.tensanchitiet,
                LoaiSan = x.maloaisanNavigation?.tenloaisan,
                GiaBuoiSang = x.giathuebuoisang,
                GiaBuoiToi = x.giathuebuoitoi
            }).ToList();
        }

        public async Task<bool> CapNhatGia(int maSanChiTiet, CapNhatGiaDTO dto)
        {
            var san = await _repo.GetById(maSanChiTiet);

            if (san == null) return false;

            san.giathuebuoisang = dto.GiaBuoiSang;
            san.giathuebuoitoi = dto.GiaBuoiToi;

            await _repo.Save();

            return true;
        }
    }
}