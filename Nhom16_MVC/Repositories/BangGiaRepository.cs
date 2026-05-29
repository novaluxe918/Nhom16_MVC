using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Repositories.Interfaces;

namespace Nhom16_MVC.Repositories
{
    public class BangGiaRepository : IBangGiaRepository
    {
        private readonly AppDbContext _context;

        public BangGiaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<sanbongchitiet>> GetByChuSan(int chuSanId)
        {
            return await _context.sanbongchitiet
                .Include(x => x.masanbongNavigation)
                .Where(x => x.masanbongNavigation.chusan == chuSanId)
                .ToListAsync();
        }

        public async Task<sanbongchitiet?> GetById(int id)
        {
            return await _context.sanbongchitiet
                .FirstOrDefaultAsync(x => x.masanchitiet == id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}