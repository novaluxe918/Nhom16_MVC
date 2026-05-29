using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.DTOs;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class FinancialManagementService
    {
        private readonly DatabaseService _dbService;
        private readonly EmailHelper _emailHelper;

        public FinancialManagementService(DatabaseService dbService, EmailHelper emailHelper)
        {
            _dbService = dbService;
            _emailHelper = emailHelper;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách lệnh rút tiền đang ở trạng thái Chờ Xử Lý
        /// </summary>
        public async Task<List<AdminWithdrawalRequestViewDto>> GetPendingWithdrawalsAsync()
        {
            var list = new List<AdminWithdrawalRequestViewDto>();

            string query = @"
                SELECT y.mayeucau, y.manguoidung, n.hoten, n.email, 
                       CASE WHEN n.vaitro::text = 'chuSan' THEN 'Chủ sân' ELSE 'Người thuê sân' END as vaitro_text,
                       y.sotien, y.tennganhang, y.sotaikhoan, y.thoigianrut
                FROM public.yeucauruttien y
                JOIN public.nguoidung n ON y.manguoidung = n.manguoidung
                WHERE y.trangthai::text = 'cho_xu_ly'
                ORDER BY y.mayeucau DESC";

            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string nganHang = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    string soTK = reader.IsDBNull(7) ? "" : reader.GetString(7);

                    list.Add(new AdminWithdrawalRequestViewDto
                    {
                        MaYeuCau = reader.GetInt32(0),
                        MaNguoiDung = reader.GetInt32(1),
                        TenNguoiDung = reader.GetString(2),
                        Email = reader.GetString(3),
                        VaiTro = reader.GetString(4),
                        SoTienRut = reader.GetInt64(5), // Cột 'sotien' kiểu bigint (long) trong database
                        TrangThai = "Chờ xử lý",
                        ThongTinNganHang = $"{nganHang} - STK: {soTK}",
                        CreatedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("🚨 LỖI TRUY VẤN DÒNG TIỀN ADMIN: " + ex.Message);
                Console.WriteLine("--------------------------------------------------");
            }
            return list;
        }

        /// <summary>
        /// Admin phê duyệt giải ngân (da_chuyen -> Trừ tiền ví) hoặc Từ chối (that_bai -> Giữ nguyên tiền ví)
        /// </summary>
        public async Task<FinancialManagementResponse> ProcessWithdrawalAsync(ProcessWithdrawalRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                string userEmail = string.Empty;
                string userName = string.Empty;
                long soTienRut = 0;

                // 1. Kiểm tra xem lệnh rút tiền có tồn tại hay không và lấy thông tin người dùng
                string checkQuery = @"
                    SELECT y.sotien, n.email, n.hoten 
                    FROM public.yeucauruttien y
                    JOIN public.nguoidung n ON y.manguoidung = n.manguoidung
                    WHERE y.mayeucau = @maYeuCau";

                using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@maYeuCau", request.MaYeuCau);
                    using var reader = await checkCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        soTienRut = reader.GetInt64(0);
                        userEmail = reader.GetString(1);
                        userName = reader.GetString(2);
                    }
                    else
                    {
                        return new FinancialManagementResponse { Success = false, Message = "Yêu cầu rút tiền không tồn tại." };
                    }
                }

                // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu (Lỗi một lệnh là hủy toàn bộ)
                using var transaction = await conn.BeginTransactionAsync();
                try
                {
                    // Gán giá trị chuỗi khớp hoàn toàn với trang_thai_rut ENUM trong Postgres
                    string dbStatus = (request.TrangThaiMoi == "da_chuyen") ? "da_chuyen" : "that_bai";

                    // Bước A: Cập nhật trạng thái của lệnh rút tiền trong bảng yeucauruttien
                    string updateStatusQuery = "UPDATE public.yeucauruttien SET trangthai = @trangThai::trang_thai_rut WHERE mayeucau = @maYeuCau";
                    using (var updateCmd = new NpgsqlCommand(updateStatusQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@trangThai", dbStatus);
                        updateCmd.Parameters.AddWithValue("@maYeuCau", request.MaYeuCau);
                        await updateCmd.ExecuteNonQueryAsync();
                    }

                    // Bước B: Xử lý cộng/trừ số dư tài khoản của người dùng dựa vào kết quả duyệt
                    if (request.TrangThaiMoi == "da_chuyen")
                    {
                        // 🌟 THỰC HIỆN TRỪ TIỀN: Duyệt thành công thì tiến hành trừ tiền trực tiếp trong ví người dùng
                        string deductQuery = @"
                            UPDATE public.nguoidung 
                            SET sodutaikhoan = sodutaikhoan - @soTien 
                            WHERE manguoidung = (SELECT manguoidung FROM public.yeucauruttien WHERE mayeucau = @maYeuCau)";

                        using var deductCmd = new NpgsqlCommand(deductQuery, conn);
                        deductCmd.Parameters.AddWithValue("@soTien", soTienRut);
                        deductCmd.Parameters.AddWithValue("@maYeuCau", request.MaYeuCau);
                        await deductCmd.ExecuteNonQueryAsync();
                    }
                    else if (request.TrangThaiMoi == "tu_choi")
                    {
                        // THỰC HIỆN TỪ CHỐI: Vì lúc người dùng ấn nút tạo yêu cầu rút tiền hệ thống CHƯA trừ tiền,
                        // nên khi từ chối ta giữ nguyên số dư của họ (Không cần chạy lệnh UPDATE ví nữa).
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new FinancialManagementResponse { Success = false, Message = "Lỗi hệ thống khi cập nhật cơ sở dữ liệu: " + ex.Message };
                }

                // 2. Tự động bắn Email thông báo giao dịch tài chính về hòm thư người dùng
                if (!string.IsNullOrEmpty(userEmail))
                {
                    string subject = request.TrangThaiMoi == "da_chuyen"
                        ? "💸 SportSync: Lệnh rút tiền thành công!"
                        : "🚨 SportSync: Lệnh rút tiền bị từ chối!";

                    string statusTitle = request.TrangThaiMoi == "da_chuyen" ? "RÚT TIỀN THÀNH CÔNG" : "GIAO DỊCH BỊ TỪ CHỐI";
                    string themeColor = request.TrangThaiMoi == "da_chuyen" ? "#28a745" : "#dc3545";

                    string bodyContent = request.TrangThaiMoi == "da_chuyen"
                        ? $"<p>Xin chào <strong>{userName}</strong>,</p><p>Yêu cầu rút số tiền <strong>{soTienRut:N0} VNĐ</strong> về tài khoản ngân hàng của bạn đã được Admin phê duyệt giải ngân thành công.</p>"
                        : $"<p>Kính gửi <strong>{userName}</strong>,</p><p>Yêu cầu rút số tiền <strong>{soTienRut:N0} VNĐ</strong> đã bị từ chối.</p><p><strong>Lý do từ chối từ hệ thống:</strong> {request.LyDoTuChoi}</p><p>Số dư ví tài khoản SportSync của bạn vẫn được giữ nguyên.</p>";

                    string emailHtml = $"<html><body style='font-family: Arial; padding: 20px;'><div style='border-top: 6px solid {themeColor}; padding: 20px;'><h2>{statusTitle}</h2>{bodyContent}</div></body></html>";

                    try { await _emailHelper.SendEmailAsync(userEmail, subject, emailHtml); } catch { }
                }

                return new FinancialManagementResponse
                {
                    Success = true,
                    Message = request.TrangThaiMoi == "da_chuyen" ? "Đã phê duyệt lệnh rút tiền và trừ số dư ví thành công!" : "Đã từ chối lệnh rút tiền và giữ nguyên số dư ví!"
                };
            }
            catch (Exception ex)
            {
                return new FinancialManagementResponse { Success = false, Message = "Lỗi hệ thống tài chính: " + ex.Message };
            }
        }
    }
}