using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Repositories
{
    public class SanBongRepository : ISanBongRepository
    {
        private readonly AppDbContext _context;

        public SanBongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<sanbong>> LayTatCa()
        {
            return await _context.sanbong
                .OrderByDescending(x => x.createdat)
                .ToListAsync();
        }

        public async Task<sanbong?> LayTheoId(int id)
        {
            return await _context.sanbong
                .FirstOrDefaultAsync(x => x.masanbong == id);
        }

        public async Task Tao(sanbong san)
        {
            await _context.sanbong.AddAsync(san);
        }

        public void CapNhat(sanbong san)
        {
            _context.sanbong.Update(san);
        }

        public void Xoa(sanbong san)
        {
            _context.sanbong.Remove(san);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
