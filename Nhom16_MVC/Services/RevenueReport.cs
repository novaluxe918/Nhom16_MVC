using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Services
{
    public class RevenueReportService : IReportService
    {
        private readonly AppDbContext _context;

        public RevenueReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RevenueReportDto>>
            GetRevenueReport(int chuSanId)
        {
            var result = await _context.sanbong
                .Where(sb => sb.chusan == chuSanId)
                .Select(sb => new RevenueReportDto
                {
                    MaSanBong = sb.masanbong,

                    TenSan = sb.tensan,

                    TongLuotDat = sb.sanbongchitiet
                        .SelectMany(sct => sct.chitietdatsan)
                        .Count(ct =>
                            ct.trangthaidatsan ==
                            TrangThaiDatEnum.HoanThanh
                        ),

                    TongDoanhThu = sb.sanbongchitiet
                        .SelectMany(sct => sct.chitietdatsan)
                        .Where(ct =>
                            ct.trangthaidatsan ==
                            TrangThaiDatEnum.HoanThanh
                        )
                        .Sum(ct =>
                            (long?)ct.masanchitietNavigation
                                .giathuebuoitoi
                        ) ?? 0
                })
                .ToListAsync();

            return result;
        }
    }
}