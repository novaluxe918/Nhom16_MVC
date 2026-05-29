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

        /// <summary>
        /// Lấy danh sách toàn bộ đánh giá hệ thống (Đã sửa đổi tên cột khớp 100% với Entity danhgia.cs)
        /// </summary>
        public async Task<List<AdminRatingViewDto>> GetAllRatingsAsync()
        {
            var list = new List<AdminRatingViewDto>();

            // 🌟 ĐÃ SỬA: Thay đổi sang d.nguoithue, d.diemso, d.binhluan, d.thoigiandanhgia cho chuẩn khớp database
            string query = @"
                SELECT d.madanhgia, d.masanbong, s.tensan, n.hoten, d.diemso, d.binhluan, d.thoigiandanhgia
                FROM public.danhgia d
                JOIN public.sanbong s ON d.masanbong = s.masanbong
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
                        MaDanhGia = reader.GetInt32(0),
                        MaSanBong = reader.GetInt32(1),
                        TenSan = reader.GetString(2),
                        TenNguoiDung = reader.GetString(3),
                        SoSao = reader.GetInt16(4), // Đã ép kiểu sang short (Int16) theo đúng thực thể
                        NoiDung = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        CreatedAt = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy danh sách đánh giá hệ thống: " + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// Xóa bỏ đánh giá dựa theo mã đánh giá chuẩn xác
        /// </summary>
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
                if (rowsAffected == 0)
                {
                    return new RatingManagementResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy mã đánh giá này hoặc dữ liệu đã bị gỡ bỏ trước đó."
                    };
                }

                return new RatingManagementResponse
                {
                    Success = true,
                    Message = "Đã gỡ bỏ đánh giá spam không phù hợp khỏi hệ thống thành công!"
                };
            }
            catch (Exception ex)
            {
                return new RatingManagementResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống phát sinh khi thực hiện xóa đánh giá: " + ex.Message
                };
            }
        }
    }
}