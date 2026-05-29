using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class DanhGiaService
    {
        private readonly AppDbContext _context;

        public DanhGiaService(AppDbContext context)
        {
            _context = context;
        }

        //Lấy danh sách bình luận của sân con 
        public async Task<DanhSachBinhLuanPhanTrangDto> GetBinhLuanSanConAsync(int maSanChiTiet, int page)
        {
            int pageSize = 5; // Load 5 bình luận mỗi lần
            var result = new DanhSachBinhLuanPhanTrangDto { TrangHienTai = page };

            var query = _context.danhgia
                .Include(d => d.nguoithueNavigation)
                .Where(d => d.masanchitiet == maSanChiTiet);

            int tongSoBinhLuan = await query.CountAsync();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoBinhLuan / pageSize);

            result.TongSoBinhLuan = tongSoBinhLuan;
            result.TongSoTrang = tongSoTrang < 1 ? 1 : tongSoTrang;

            result.ListBinhLuan = await query
                .OrderByDescending(d => d.thoigiandanhgia) // Mới nhất lên đầu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new BinhLuanHienThiDto
                {
                    MaDanhGia = d.madanhgia,
                    TenNguoiDung = d.nguoithueNavigation.hoten,
                    AvatarNguoiDung = d.nguoithueNavigation.avatar,
                    DiemSo = d.diemso,
                    BinhLuan = d.binhluan,
                    ThoiGian = d.thoigiandanhgia.HasValue ? d.thoigiandanhgia.Value.ToString("dd/MM/yyyy HH:mm") : ""
                })
                .ToListAsync();

            return result;
        }

        //lưu đánh giá
        public async Task<searchSuggestResponse> LuuDanhGiaCuaKhachAsync(int maNguoiDung, DanhGiaRequestDto request)
        {
            var response = new searchSuggestResponse { Success = false };

            if (request.DiemSo < 1 || request.DiemSo > 5)
            {
                response.Message = "Số sao đánh giá phải từ 1 đến 5.";
                return response;
            }

            var danhGiaMoi = new danhgia
            {
                masanchitiet = request.MaSanChiTiet,
                nguoithue = maNguoiDung,
                diemso = request.DiemSo,
                binhluan = request.BinhLuan,
                thoigiandanhgia = DateTime.Now
            };

            try
            {
                await _context.danhgia.AddAsync(danhGiaMoi);
                await _context.SaveChangesAsync(); 

                response.Success = true;
                response.Message = "Gửi đánh giá thành công! Cảm ơn bạn đã phản hồi.";
                return response;
            }
            catch (DbUpdateException ex)
            {
                
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23P01")
                {
                    response.Message = pgEx.MessageText; 
                }
                else
                {
                    response.Message = $"Lỗi ghi dữ liệu: {ex.InnerException?.Message ?? ex.Message}";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi hệ thống: {ex.Message}";
                return response;
            }
        }
    }
}
