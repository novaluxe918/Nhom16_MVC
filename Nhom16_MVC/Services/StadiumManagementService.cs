using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.DTOs;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class StadiumManagementService
    {
        private readonly DatabaseService _dbService;
        private readonly EmailHelper _emailHelper;

        public StadiumManagementService(DatabaseService dbService, EmailHelper emailHelper)
        {
            _dbService = dbService;
            _emailHelper = emailHelper;
        }

        public async Task<List<StadiumApprovalViewDto>> GetUnapprovedStadiumsAsync()
        {
            var list = new List<StadiumApprovalViewDto>();
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 🌟 ĐÃ SỬA: s.chusan và s.hinhanh để khớp chuẩn xác 100% với Entity sanbong.cs của bạn
                string query = @"
                    SELECT s.masanbong, s.tensan, n.hoten, s.diachi, s.mota, s.hinhanh, s.daduyet
                    FROM sanbong s
                    JOIN nguoidung n ON s.chusan = n.manguoidung
                    WHERE s.daduyet = false OR s.daduyet IS NULL
                    ORDER BY s.masanbong DESC";

                var dictionary = new Dictionary<int, StadiumApprovalViewDto>();

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int maSan = reader.GetInt32(0);
                        var dto = new StadiumApprovalViewDto
                        {
                            MaSanBong = maSan,
                            TenSan = reader.GetString(1),
                            ChuSan = reader.GetString(2),
                            DiaChi = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            MoTa = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            HinhAnhDaiDien = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            DaDuyet = reader.IsDBNull(6) ? false : reader.GetBoolean(6),
                            DanhSachHinhAnhChiTiet = new List<string>()
                        };
                        dictionary[maSan] = dto;
                        list.Add(dto);
                    }
                }

                // Lấy mảng ảnh chi tiết từ bảng media_sanbong
                if (list.Count > 0)
                {
                    string mediaQuery = "SELECT masanbong, link FROM media_sanbong WHERE masanbong = ANY(@ids)";
                    using var mediaCmd = new NpgsqlCommand(mediaQuery, conn);
                    mediaCmd.Parameters.AddWithValue("@ids", list.Select(x => x.MaSanBong).ToArray());

                    using var readerMedia = await mediaCmd.ExecuteReaderAsync();
                    while (await readerMedia.ReadAsync())
                    {
                        int maSan = readerMedia.GetInt32(0);
                        string linkAnh = readerMedia.GetString(1);
                        if (dictionary.ContainsKey(maSan))
                        {
                            dictionary[maSan].DanhSachHinhAnhChiTiet.Add(linkAnh);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy danh sách sân chờ duyệt: " + ex.Message);
            }
            return list;
        }

        public async Task<StadiumApprovalResponse> ProcessStadiumApprovalAsync(ApproveStadiumRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                string emailChuSan = string.Empty;
                string tenChuSan = string.Empty;
                string tenSan = string.Empty;

                // 🌟 ĐÃ SỬA: s.chusan khớp với bảng
                var infoQuery = @"
                    SELECT s.tensan, n.hoten, n.email 
                    FROM sanbong s
                    JOIN nguoidung n ON s.chusan = n.manguoidung
                    WHERE s.masanbong = @id";

                using (var infoCmd = new NpgsqlCommand(infoQuery, conn))
                {
                    infoCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    using var reader = await infoCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        tenSan = reader.GetString(0);
                        tenChuSan = reader.GetString(1);
                        emailChuSan = reader.GetString(2);
                    }
                    else
                    {
                        return new StadiumApprovalResponse { Success = false, Message = "Không tìm thấy dữ liệu sân bóng." };
                    }
                }

                if (request.IsApproved)
                {
                    var updateQuery = "UPDATE sanbong SET daduyet = true WHERE masanbong = @id";
                    using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    await updateCmd.ExecuteNonQueryAsync();
                }
                else
                {
                    // Xóa các bảng phụ trước để tránh dính khóa ngoại khi từ chối sân ảo
                    var deleteMediaQuery = "DELETE FROM media_sanbong WHERE masanbong = @id";
                    using var deleteMediaCmd = new NpgsqlCommand(deleteMediaQuery, conn);
                    deleteMediaCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    await deleteMediaCmd.ExecuteNonQueryAsync();

                    var deleteStadiumQuery = "DELETE FROM sanbong WHERE masanbong = @id";
                    using var deleteStadiumCmd = new NpgsqlCommand(deleteStadiumQuery, conn);
                    deleteStadiumCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    await deleteStadiumCmd.ExecuteNonQueryAsync();
                }

                // Gửi Mail HTML mẫu chuẩn phản hồi đối tác
                if (!string.IsNullOrEmpty(emailChuSan))
                {
                    string emailSubject = request.IsApproved
                        ? "🎉 THÔNG BÁO: Cơ sở sân bóng của bạn đã được PHÊ DUYỆT!"
                        : "🚨 THÔNG BÁO: Yêu cầu đăng ký sân bóng bị TỪ CHỐI!";

                    string themeColor = request.IsApproved ? "#28a745" : "#dc3545";
                    string statusTitle = request.IsApproved ? "ĐĂNG KÝ ĐƯỢC PHÊ DUYỆT" : "YÊU CẦU BỊ TỪ CHỐI";

                    string contentBody = request.IsApproved
                        ? $"<p>Xin chào đối tác <strong>{tenChuSan}</strong>,</p><p>Cơ sở sân bóng <strong>{tenSan}</strong> của bạn đã vượt qua kiểm duyệt và chính thức được <strong>KÍCH HOẠT</strong> hoạt động công khai trên hệ thống <strong>SportSync</strong>.</p>"
                        : $"<p>Kính gửi đối tác <strong>{tenChuSan}</strong>,</p><p>Yêu cầu đăng ký sân bóng <strong>{tenSan}</strong> của bạn đã bị ban quản trị <strong>TỪ CHỐI CHẤP THUẬN</strong>.</p>" +
                          $"<div style='background-color: #fff5f5; border-left: 4px solid #dc3545; padding: 15px; margin: 20px 0;'>" +
                          $"<strong style='color: #dc3545;'>Lý do từ chối:</strong><br/><p style='margin: 5px 0 0 0; color: #333;'>{request.LyDoTuChoi}</p></div>";

                    string emailHtml = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
                            <div style='background-color: white; max-width: 600px; margin: 0 auto; padding: 30px; border-radius: 8px; border-top: 6px solid {themeColor}; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                                <h2 style='color: {themeColor}; text-align: center; margin-top: 0;'>{statusTitle}</h2>
                                <hr style='border: none; border-top: 1px solid #eee; margin: 15px 0;'>
                                {contentBody}
                                <hr style='border: none; border-top: 1px solid #eee; margin: 25px 0;'>
                                <p style='color: #888; font-size: 12px; text-align: center;'>Đây là email tự động từ hệ thống quản trị SportSync.</p>
                            </div>
                        </body>
                    </html>";

                    try { await _emailHelper.SendEmailAsync(emailChuSan, emailSubject, emailHtml); } catch { }
                }

                return new StadiumApprovalResponse
                {
                    Success = true,
                    Message = request.IsApproved ? "Đã phê duyệt sân bóng thành công!" : "Đã từ chối và gỡ bỏ thông tin sân thành công!"
                };
            }
            catch (Exception ex)
            {
                return new StadiumApprovalResponse { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" };
            }
        }
    }
}