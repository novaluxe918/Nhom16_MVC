using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Repositories;

namespace Nhom16_MVC.Services
{
    public class SanBongService : ISanBongService
    {
        private readonly ISanBongRepository _repository;

        public SanBongService(ISanBongRepository repository)
        {
            _repository = repository;
        }
        //public async Task<ChiTietSanMeDto> GetChiTietSanMeAsync(int maSanMenge)
        //{
        //    var sanBong = await _db.sanbong
        //        .Include(s => s.media_sanbong)
        //        .Include(s => s.sanbongchitiet)
        //            .ThenInclude(sc => sc.maloaisanNavigation)
        //        .Include(s => s.sanbongchitiet)
        //            .ThenInclude(sc => sc.media_sanbongchitiet)
        //        .FirstOrDefaultAsync(s => s.masanbong == maSanMenge && s.daduyet == true);

        //    if (sanBong == null) return null;

        //    //var firstSubPitch =  sanBong.sanbongchitiet.FirstOrDefault();
        //    string gioHoatDong = $"{sanBong.giomocua:HH:mm} - {sanBong.giodongcua:HH:mm}";

        //    return new ChiTietSanMeDto
        //    {
        //        MaSanBong = sanBong.masanbong,
        //        TenSan = sanBong.tensan,
        //        MoTa = sanBong.mota,
        //        DiaChi = sanBong.diachi,
        //        Quan = sanBong.quan,
        //        ThanhPhos = sanBong.thanhpho,
        //        KinhDo = sanBong.kinhdo,
        //        ViDo = sanBong.vido,
        //        GioHoatDong = gioHoatDong, // Nhận giá trị chuỗi cấu hình chuẩn từ Sân mẹ
        //        AlbumMedia = sanBong.media_sanbong.Select(m => m.link).ToList(),
        //        DanhSachSanCon = sanBong.sanbongchitiet.Select(sc => new SanConTrongSanMeDto
        //        {
        //            MaSanChiTiet = sc.masanchitiet,
        //            TenSanChiTiet = sc.tensanchitiet,
        //            LoaiSan = sc.maloaisanNavigation?.tenloaisan,
        //            GiaThueBuoiSang = sc.giathuebuoisang,
        //            GiaThueBuoiToi = sc.giathuebuoitoi,
        //            AnhDaiDien = sc.media_sanbongchitiet.FirstOrDefault(m => m.loaimedia == "hinh_anh")?.link ?? sanBong.hinhanh
        //        }).ToList()
        //    };
        //}

        public async Task<ChiTietSanMeDto> GetChiTietSanMeAsync(int maSanMenge)
        {
            var sanBong = await _db.sanbong
                .Include(s => s.media_sanbong)
                .Include(s => s.sanbongchitiet)
                    .ThenInclude(sc => sc.maloaisanNavigation)
                .Include(s => s.sanbongchitiet)
                    .ThenInclude(sc => sc.media_sanbongchitiet)
                .FirstOrDefaultAsync(s => s.masanbong == maSanMenge && s.daduyet == true);

            if (sanBong == null) return null;

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
                GioHoatDong = gioHoatDong,
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

            await _repository.Tao(san);
            await _repository.Save();

            return true;
        }

        public async Task<bool> CapNhatSan(int id, CapNhatSanBongDTO dto)
        {
            var san = await _repository.LayTheoId(id);

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
            san.updatedat = DateTime.UtcNow;

            _repository.CapNhat(san);

            await _repository.Save();

            return true;
        }

        // Lấy danh sách Sân Mẹ (có hỗ trợ tìm theo tên)
        public async Task<List<ChiTietSanMeDto>> GetDanhSachSanMeAsync(string? keyword)
        {
            var query = _db.sanbong
                .Include(s => s.sanbongchitiet)
                .Where(s => s.daduyet == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s => s.tensan.ToLower().Contains(keyword.ToLower()));
            }

            var sanBongs = await query.ToListAsync();

            return sanBongs.Select(s => new ChiTietSanMeDto
            {
                MaSanBong = s.masanbong,
                TenSan = s.tensan,
                DiaChi = s.diachi,
                Quan = s.quan,
                // Đếm xem cụm này có bao nhiêu sân con
                SoLuongSanCon = s.sanbongchitiet.Count,
                // Lấy ảnh gốc của sân mẹ
                AlbumMedia = new List<string> { s.hinhanh }
            }).ToList();
        }
    }
}
