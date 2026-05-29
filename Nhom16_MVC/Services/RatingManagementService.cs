using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nhom16_MVC.Models.DTOs;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class RatingManagementService
    {
        private readonly DatabaseService _dbService;

        public RatingManagementService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<List<AdminRatingViewDto>> GetAllRatingsAsync()
        {
            var list = new List<AdminRatingViewDto>();

            // 🚀 ĐÃ SỬA: Thay d.masanbong bằng sbc.masanbong và thêm JOIN public.sanbongchitiet sbc
            string query = @"
                SELECT d.madanhgia, sbc.masanbong, s.tensan, n.hoten, d.diemso, d.binhluan, d.thoigiandanhgia
                FROM public.danhgia d
                JOIN public.sanbongchitiet sbc ON d.masanchitiet = sbc.masanchitiet
                JOIN public.sanbong s ON sbc.masanbong = s.masanbong
                JOIN public.nguoidung n ON d.nguoithue = n.manguoidung
                ORDER BY d.madanhgia DESC";

            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new AdminRatingViewDto
                    {
                        MaDanhGia = Convert.ToInt32(reader["madanhgia"]),
                        MaSanBong = Convert.ToInt32(reader["masanbong"]), // Lấy từ sbc.masanbong vừa select
                        TenSan = reader["tensan"]?.ToString() ?? string.Empty,
                        TenNguoiDung = reader["hoten"]?.ToString() ?? string.Empty,
                        SoSao = Convert.ToInt32(reader["diemso"]),
                        NoiDung = reader["binhluan"]?.ToString() ?? string.Empty,
                        CreatedAt = reader["thoigiandanhgia"] != DBNull.Value ? Convert.ToDateTime(reader["thoigiandanhgia"]) : null
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                // Giữ lại throw để nếu có lỗi phát sinh khác (ví dụ sai tên cột người dùng) thì Postman sẽ hiện ngay
                throw new Exception($"Lỗi truy vấn Database: {ex.Message}", ex);
            }
        }

        public async Task<RatingManagementResponse> DeleteRatingAsync(int maDanhGia)
        {
            string query = "DELETE FROM public.danhgia WHERE madanhgia = @maDanhGia";
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@maDanhGia", maDanhGia);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0
                    ? new RatingManagementResponse { Success = true, Message = "Đã xóa thành công!" }
                    : new RatingManagementResponse { Success = false, Message = "Không tìm thấy đánh giá." };
            }
            catch (Exception ex)
            {
                return new RatingManagementResponse { Success = false, Message = ex.Message };
            }
        }
    }
}