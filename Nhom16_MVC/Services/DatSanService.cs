using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Services.Interfaces;

namespace Nhom16_MVC.Services
{
    public class DatSanService : IDatSanService
    {
        private readonly AppDbContext _context;

        public DatSanService(AppDbContext context)
        {
            _context = context;
        }

        public List<LichDatSanDto> GetLichDat(FilterLichDatDto filter)
        {
            var query = from ct in _context.chitietdatsan
                        join ds in _context.datsan on ct.madatsan equals ds.madatsan
                        join sbc in _context.sanbongchitiet on ct.masanchitiet equals sbc.masanchitiet
                        join sb in _context.sanbong on sbc.masanbong equals sb.masanbong
                        join nd in _context.nguoidung on ds.nguoithue equals nd.manguoidung
                        where sb.chusan == filter.ChuSanId
                        select new LichDatSanDto
                        {
                            MaDatSan = ds.madatsan,
                            TenNguoiDat = nd.hoten,
                            TenSanCon = sbc.tensanchitiet,
                            NgayDat = ds.ngaydat,
                            GioBatDau = ct.giobatdau,
                            GioKetThuc = ct.gioketthuc,
                            TrangThai = ct.trangthaidatsan.ToString()
                        };

            if (filter.TuNgay.HasValue)
                query = query.Where(x => x.NgayDat >= filter.TuNgay.Value);

            if (filter.DenNgay.HasValue)
                query = query.Where(x => x.NgayDat <= filter.DenNgay.Value);

            if (!string.IsNullOrEmpty(filter.TrangThai))
                query = query.Where(x => x.TrangThai == filter.TrangThai);

            if (filter.MaSanChiTiet.HasValue)
                query = query.Where(x => x.TenSanCon != null);

            return query
                .OrderByDescending(x => x.NgayDat)
                .ToList();
        }
    }
}