using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Repositories
{
    public class DatSanRepository : IDatSanRepository
    {
        private readonly AppDbContext _context;

        public DatSanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DatSanResponseDTO>> GetLichDatByChuSan(
            int chusan,
            FilterDatSanDTO filter)
        {
            var query = _context.chitietdatsans
                .Include(x => x.madatsanNavigation)
                    .ThenInclude(x => x.nguoithueNavigation)
                .Include(x => x.masanchitietNavigation)
                    .ThenInclude(x => x.masanbongNavigation)
                .AsQueryable();

            query = query.Where(x =>
                x.masanchitietNavigation
                 .masanbongNavigation
                 .chusan == chusan);

            // lọc ngày
            if (filter.ngay.HasValue)
            {
                query = query.Where(x =>
                    DateOnly.FromDateTime(x.giobatdau) == filter.ngay.Value);
            }

            // lọc trạng thái
            if (!string.IsNullOrEmpty(filter.trangThai))
            {
                query = query.Where(x =>
                    x.trangthaidatsan.ToString() == filter.trangThai);
            }

            // lọc sân con
            if (filter.maSanChiTiet.HasValue)
            {
                query = query.Where(x =>
                    x.masanchitiet == filter.maSanChiTiet.Value);
            }

            return await query
                .OrderBy(x => x.giobatdau)
                .Select(x => new DatSanResponseDTO
                {
                    maChiTietDatSan = x.machitietdatsan,

                    maDatSan = x.madatsan,

                    tenNguoiDat =
                        x.madatsanNavigation
                         .nguoithueNavigation
                         .hoten,

                    tenSan =
                        x.masanchitietNavigation
                         .masanbongNavigation
                         .tensan,

                    tenSanChiTiet =
                        x.masanchitietNavigation
                         .tensanchitiet,

                    gioBatDau = x.giobatdau,

                    gioKetThuc = x.gioketthuc,

                    trangThaiDatSan =
                        x.trangthaidatsan.ToString(),

                    coVanDe = x.covande ?? false
                })
                .ToListAsync();
        }
    }
}