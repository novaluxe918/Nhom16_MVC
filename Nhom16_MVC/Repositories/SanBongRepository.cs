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

        public async Task<List<sanbong>> GetAllAsync()
        {
            return await _context.sanbong
                .Include(x => x.sanbongchitiet)
                .ToListAsync();
        }

        public async Task<List<sanbong>> GetByChuSanAsync(int chusan)
        {
            return await _context.sanbong
                .Where(x => x.chusan == chusan)
                .Include(x => x.sanbongchitiet)
                .ToListAsync();
        }

        public async Task<sanbong?> GetByIdAsync(int id)
        {
            return await _context.sanbong
                .Include(x => x.sanbongchitiet)
                .Include(x => x.media_sanbong)
                .FirstOrDefaultAsync(x => x.masanbong == id);
        }

        public async Task AddAsync(sanbong san)
        {
            await _context.sanbong.AddAsync(san);
        }

        public Task UpdateAsync(sanbong san)
        {
            _context.sanbong.Update(san);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(sanbong san)
        {
            _context.sanbong.Remove(san);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
