using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Enums;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class UserManagementService
    {
        private readonly DatabaseService _dbService;
        private readonly EmailHelper _emailHelper;

        public UserManagementService(DatabaseService dbService, EmailHelper emailHelper)
        {
            _dbService = dbService;
            _emailHelper = emailHelper;
        }

        // 1. Lấy danh sách tài khoản
        // 1. Lấy danh sách tài khoản
        public async Task<UserManagementResponse> GetAllUsersAsync(string? roleFilter = null)
        {
            var users = new List<UserDto>();
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 🌟 SỬA TẠI ĐÂY: Thay "vaitro != 0" bằng "vaitro != 'admin'" (So sánh dạng chuỗi ENUM)
                string query = @"
            SELECT manguoidung, hoten, email, sodienthoai, vaitro, sodutaikhoan, isemailverified, trangthai 
            FROM public.nguoidung 
            WHERE vaitro != 'admin'";

                if (!string.IsNullOrEmpty(roleFilter))
                {
                    query += " AND vaitro = @role";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(roleFilter))
                    {
                        // 🌟 SỬA TẠI ĐÂY: Truyền trực tiếp giá trị chuỗi khớp với ENUM trong Database Postgres
                        string roleValue = roleFilter.ToLower() == "chusan" ? "chuSan" : "nguoiThue";
                        cmd.Parameters.AddWithValue("@role", roleValue);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // 🌟 SỬA TẠI ĐÂY: Vì Postgres trả về chuỗi ('chuSan'/'nguoiThue') nên dùng reader.GetString(4)
                            string roleDbValue = reader.GetString(4);
                            string roleText = roleDbValue == "chuSan" ? "Chủ Sân" : "Người Thuê";

                            users.Add(new UserDto
                            {
                                MaNguoiDung = reader.GetInt32(0),
                                HoTen = reader.GetString(1),
                                Email = reader.GetString(2),
                                SoDienThoai = reader.IsDBNull(3) ? null : reader.GetString(3),
                                VaiTro = roleText,
                                SoDuTaiKhoan = reader.GetInt64(5),
                                IsEmailVerified = reader.IsDBNull(6) ? false : reader.GetBoolean(6),
                                TrangThai = reader.IsDBNull(7) ? "hoat_dong" : reader.GetString(7)
                            });
                        }
                    }
                }

                return new UserManagementResponse
                {
                    Success = true,
                    Message = "Lấy danh sách người dùng thành công.",
                    Data = users
                };
            }
            catch (Exception ex)
            {
                return new UserManagementResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống: " + ex.Message,
                    Data = null
                };
            }
        }

        // 2. Khóa hoặc Mở khóa tài khoản
        public async Task<UserManagementResponse> ToggleUserLockAsync(ToggleLockDto dto)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 🌟 Kiểm tra xem người dùng có tồn tại không và lấy thông tin gửi email
                string checkQuery = "SELECT hoten, email FROM public.nguoidung WHERE manguoidung = @maNguoiDung";
                string targetEmail = string.Empty;
                string targetName = string.Empty;

                using (var cmdCheck = new NpgsqlCommand(checkQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@maNguoiDung", dto.MaNguoiDung);
                    using (var reader = await cmdCheck.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            targetName = reader.GetString(0);
                            targetEmail = reader.GetString(1);
                        }
                        else
                        {
                            return new UserManagementResponse { Success = false, Message = "Không tìm thấy người dùng có mã này." };
                        }
                    }
                }

                // 🌟 Xử lý cập nhật cột trangthai ('bi_khoa' hoặc 'hoat_dong')
                string targetStatus = dto.IsLocked ? "bi_khoa" : "hoat_dong";

                string updateQuery = @"
                    UPDATE public.nguoidung 
                    SET trangthai = @trangThai 
                    WHERE manguoidung = @maNguoiDung";

                using (var cmdUpdate = new NpgsqlCommand(updateQuery, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@trangThai", targetStatus);
                    cmdUpdate.Parameters.AddWithValue("@maNguoiDung", dto.MaNguoiDung);
                    await cmdUpdate.ExecuteNonQueryAsync();
                }

                // 🌟 Nếu hành động là KHÓA TÀI KHOẢN -> Tiến hành biên soạn mẫu Mail gửi thông báo kỷ luật
                if (dto.IsLocked)
                {
                    string reasonText = "Tài khoản của bạn tạm thời bị khóa do vi phạm tiêu chuẩn cộng đồng của hệ thống.";
                    if (dto.LockReasonType.ToLower() == "spam")
                    {
                        reasonText = "Hệ thống phát hiện tài khoản của bạn liên tục có hành vi đặt sân ảo, hủy lịch bừa bãi hoặc spam tin nhắn làm phiền người dùng khác.";
                    }
                    else if (dto.LockReasonType.ToLower() == "gia_mao")
                    {
                        reasonText = "Tài khoản này bị tố cáo hoặc bị hệ thống quét thấy thông tin cơ sở sân bóng hoặc danh tính có dấu hiệu lừa đảo, giả mạo tổ chức khác.";
                    }
                    else if (dto.LockReasonType.ToLower() == "khac" && !string.IsNullOrEmpty(dto.CustomReason))
                    {
                        reasonText = dto.CustomReason;
                    }

                    // Gửi Email mẫu giao diện đỏ cảnh báo nghiêm khắc
                    string emailBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
                            <div style='background-color: white; max-width: 600px; margin: 0 auto; padding: 30px; border-radius: 8px; border-top: 6px solid #dc3545; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                                <h2 style='color: #dc3545; text-align: center; margin-top: 0;'>THÔNG BÁO KHÓA TÀI KHOẢN</h2>
                                <p>Kính gửi <strong>{targetName}</strong>,</p>
                                <p>Hệ thống quản trị nền tảng đặt sân thể thao <strong>SportSync</strong> xin thông báo: Tài khoản của bạn hiện tại đã bị tạm khóa (vô hiệu hóa) quyền truy cập công khai.</p>
                                
                                <div style='background-color: #fff5f5; border-left: 4px solid #dc3545; padding: 15px; margin: 20px 0;'>
                                    <strong style='color: #dc3545;'>Lý do xử lý kỷ luật:</strong><br/>
                                    <p style='margin: 5px 0 0 0; color: #333; line-height: 1.5;'>{reasonText}</p>
                                </div>

                                <p>Do đó, bạn sẽ không thể thực hiện đăng nhập, đặt sân, hoặc quản lý cơ sở sân bóng cho đến khi lệnh khóa được gỡ bỏ.</p>
                                <p>Nếu bạn cho rằng đây là một sự nhầm lẫn hoặc có nhu cầu cần khiếu nại, vui lòng liên hệ ngay với Ban quản trị qua email hỗ trợ của hệ thống.</p>
                                <hr style='border: none; border-top: 1px solid #eee; margin: 25px 0;'>
                                <p style='color: #888; font-size: 12px; text-align: center;'>
                                    Đây là email tự động từ hệ thống quản trị SportSync. Vui lòng không phản hồi trực tiếp vào thư này.
                                </p>
                            </div>
                        </body>
                    </html>";

                    try
                    {
                        await _emailHelper.SendEmailAsync(targetEmail, "🚨 THÔNG BÁO KHÓA TÀI KHOẢN - SportSync", emailBody);
                    }
                    catch (Exception ex)
                    {
                        return new UserManagementResponse { Success = true, Message = $"Đã chuyển tài khoản sang trạng thái bị khóa nhưng gửi email thất bại: {ex.Message}" };
                    }

                    return new UserManagementResponse { Success = true, Message = "Khóa tài khoản và đã gửi email thông báo kỷ luật đến người dùng thành công!" };
                }

                return new UserManagementResponse { Success = true, Message = "Đã mở khóa, tài khoản quay lại trạng thái hoạt động bình thường." };
            }
            catch (Exception ex)
            {
                return new UserManagementResponse { Success = false, Message = "Lỗi hệ thống: " + ex.Message };
            }
        }
    }
}