using Nhom16_MVC.Data;
using Microsoft.EntityFrameworkCore;


namespace Nhom16_MVC.Services
{
    public class SearchService
    {
        private readonly AppDbContext _db;

        public SearchService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<string>> GetSuggestionsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<string>();
            //nếu query <2 => ds rỗng 
            if (query.Trim().Length < 2)
                return new List<string>();
            try
            {
                var trimmedQuery = query.Trim();
                var suggestions = await _db.sanbong
                    .Where(s => s.daduyet == true)
                    .Where(s => EF.Functions.ILike(s.tensan, $"%{trimmedQuery}%"))
                    .Select(s => s.tensan)
                    .Take(7)
                    .ToListAsync();
                return suggestions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tìm kiếm sân bóng: {ex.Message}");
                return new List<string>();
            }
        }

        internal async Task GetDanhSachLoaiSanAsync()
        {
            throw new NotImplementedException();
        }
    }
}
