using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Repositories;

namespace Nhom16_MVC.Services
{
    class SanBongService : ISanBongService
    { 
        private readonly ISanBongRepository _resposities;

        public SanBongService(ISanBongRepository repository)
        {
            _resposities = repository;
        }
      public async Task<bool> CapNhatSan(int id, UpdateSanBongDto dto)
        {
            var san = await _resposities.LayTheoId(id);

            if (san == null)
                return false;

            san.tensan = dto.TenSan;
            san.mota = dto.MoTa;
            san.diachi = dto.DiaChi;
            san.quan = dto.Quan;
            san.huyen = dto.Huyen;
            san.xa = dto.Xa;
            san.thanhpho = dto.ThanhPho;
            san.hinhanh = dto.HinhAnh;
            san.kinhdo = dto.KinhDo;
            san.vido = dto.ViDo;
           

            _resposities.CapNhat(san);

            await _resposities.Save();

            return true;
        }

        
        public async Task<List<SanBongDTO>> LayTatCa()
        {
           var data = await _resposities.LayTatCa();
           return data.Select(s => new SanBongDTO
           {
               MaSanBong = s.masanbong,
               ChuSan = s.chusan,
               TenSan = s.tensan,
                       MoTa = s.mota,
                DiaChi = s.diachi,
                Quan = s.quan,
                Huyen = s.huyen,
                Xa = s.xa,
                ThanhPho = s.thanhpho,
                HinhAnh = s.hinhanh,
                DaDuyet = s.daduyet,
                KinhDo = s.kinhdo,
                ViDo = s.vido
           }).ToList();
        }

      public async Task<SanBongDTO?> LayTheoId(int id)
        {
            var s = await _resposities.LayTheoId(id);

            if (s == null)
                return null;

            return new SanBongDTO
            {
                MaSanBong = s.masanbong,
                ChuSan = s.chusan,
                TenSan = s.tensan,
                MoTa = s.mota,
                DiaChi = s.diachi,
                Quan = s.quan,
                Huyen = s.huyen,
                Xa = s.xa,
                ThanhPho = s.thanhpho,
                HinhAnh = s.hinhanh,
                DaDuyet = s.daduyet,
                KinhDo = s.kinhdo,
                ViDo = s.vido
            };
        }

           public async Task<bool> TaoSan(CreateSanBongDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenSan))
                return false;

            var san = new sanbong
            {
                chusan = dto.ChuSan,
                tensan = dto.TenSan,
                mota = dto.MoTa,
                diachi = dto.DiaChi,
                quan = dto.Quan,
                huyen = dto.Huyen,
                xa = dto.Xa,
                thanhpho = dto.ThanhPho,
                hinhanh = dto.HinhAnh,
                kinhdo = dto.KinhDo,
                vido = dto.ViDo,
                daduyet = false,
                createdat = DateTime.Now
            };

            await _resposities.Tao(san);
            await _resposities.Save();

            return true;
        }

         public async Task<bool> XoaSan(int id)
        {
            var san = await _resposities.LayTheoId(id);

            if (san == null)
                return false;

            _resposities.Xoa(san);

            await _resposities.Save();

            return true;
        }
    }
}