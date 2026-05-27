
using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Repositories;

namespace Nhom16_MVC.Services
{
    public class SanBongService : ISanBongService
    {
        private readonly ISanBongRepository _repository;
        private readonly AppDbContext _db;
        public SanBongService(ISanBongRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<sanbong>> LayTatCaSan()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<sanbong>> LaySanTheoChuSan(int chusan)
        {
            return await _repository.GetByChuSanAsync(chusan);
        }

        public async Task<sanbong?> LaySanTheoId(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> TaoSan(TaoSanBongDTO dto, int chusan)
        {
            var san = new sanbong
            {
                chusan = chusan,
                tensan = dto.tensan,
                mota = dto.mota,
                hinhanh = dto.hinhanh,
                diachi = dto.diachi,
                quan = dto.quan,
                huyen = dto.huyen,
                xa = dto.xa,
                thanhpho = dto.thanhpho,
                kinhdo = dto.kinhdo,
                vido = dto.vido,
                giomocua = dto.giomocua,
                giodongcua = dto.giodongcua,
                daduyet = false,
                createdat = DateTime.UtcNow,
                updatedat = DateTime.UtcNow
            };

            await _repository.AddAsync(san);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CapNhatSan(int id, CapNhatSanBongDTO dto)
        {
            var san = await _repository.GetByIdAsync(id);

            if (san == null)
                return false;

            san.tensan = dto.tensan;
            san.mota = dto.mota;
            san.hinhanh = dto.hinhanh;
            san.diachi = dto.diachi;
            san.quan = dto.quan;
            san.huyen = dto.huyen;
            san.xa = dto.xa;
            san.thanhpho = dto.thanhpho;
            san.kinhdo = dto.kinhdo;
            san.vido = dto.vido;
            san.giomocua = dto.giomocua;
            san.giodongcua = dto.giodongcua;
            san.updatedat = DateTime.UtcNow;

            await _repository.UpdateAsync(san);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> XoaSan(int id)
        {
            var san = await _repository.GetByIdAsync(id);

            if (san == null)
                return false;

            await _repository.DeleteAsync(san);
            await _repository.SaveChangesAsync();

            return true;
        }




        public async Task<ChiTietSanMeDto> GetChiTietSanMeAsync(int maSanMenge)

        {
            var sanBong = await _db.sanbong
                .Include(s => s.media_sanbong)
                .Include(s=>s.giomocua)
                .Include(s=> s.giodongcua)
                .Include(s => s.sanbongchitiet)
                    .ThenInclude(sc => sc.maloaisanNavigation)
                .Include(s => s.sanbongchitiet)
                    .ThenInclude(sc => sc.media_sanbongchitiet)
                .FirstOrDefaultAsync(s => s.masanbong == maSanMenge && s.daduyet == true);

            if (sanBong == null) return null;

            //var firstSubPitch =  sanBong.sanbongchitiet.FirstOrDefault();
            string gioHoatDong = $"{sanBong.giomocua:HH:mm} - {sanBong.giodongcua:HH:mm}";

            return new ChiTietSanMeDto
            {
                MaSanBong = sanBong.masanbong,
                TenSan = sanBong.tensan,
                MoTa = sanBong.mota,
                DiaChi = sanBong.diachi,
                Quan = sanBong.quan,
                ThanhPhos = sanBong.thanhpho,
                KinhDo = sanBong.kinhdo,
                ViDo = sanBong.vido,
                GioHoatDong = gioHoatDong, // Nhận giá trị chuỗi cấu hình chuẩn từ Sân mẹ
                AlbumMedia = sanBong.media_sanbong.Select(m => m.link).ToList(),
                DanhSachSanCon = sanBong.sanbongchitiet.Select(sc => new SanConTrongSanMeDto
                {
                    MaSanChiTiet = sc.masanchitiet,
                    TenSanChiTiet = sc.tensanchitiet,
                    LoaiSan = sc.maloaisanNavigation?.tenloaisan,
                    GiaThueBuoiSang = sc.giathuebuoisang,
                    GiaThueBuoiToi = sc.giathuebuoitoi,
                    AnhDaiDien = sc.media_sanbongchitiet.FirstOrDefault(m => m.loaimedia == "hinh_anh")?.link ?? sanBong.hinhanh
                }).ToList()
            };
        }
    }
    }

