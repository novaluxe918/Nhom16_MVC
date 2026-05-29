using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Repositories
{
    public class SanBongChiTietRepository : ISanBongChiTietRepository
    {
        private readonly AppDbContext _context;

        public SanBongChiTietRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<sanbongchitiet>> GetAllAsync()
        {
            return await _context.sanbongchitiet
                .Include(x => x.masanbongNavigation)
                .ToListAsync();
        }

        public async Task<List<sanbongchitiet>> GetBySanBongIdAsync(int masanbong)
        {
            return await _context.sanbongchitiet
                .Where(x => x.masanbong == masanbong)
                .ToListAsync();
        }

        public async Task<sanbongchitiet?> GetByIdAsync(int id)
        {
            return await _context.sanbongchitiet
                .Include(x => x.masanbongNavigation)
                .FirstOrDefaultAsync(x => x.masanchitiet == id);
        }

        public async Task AddAsync(sanbongchitiet entity)
        {
            await _context.sanbongchitiet.AddAsync(entity);
        }

        public Task DeleteAsync(sanbongchitiet entity)
        {
            _context.sanbongchitiet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}