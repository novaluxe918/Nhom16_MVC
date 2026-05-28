using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Services
{
    public class SanBongService
    {
        private readonly AppDbContext _db;

        public SanBongService(AppDbContext db)
        {
            _db = db;
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
