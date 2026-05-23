using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models;
using Nhom16_MVC.Models.DTOs;
using Npgsql;
using System;
using System.Collections.Generic; // Thêm để dùng Dictionary, List
using System.Linq;               // Thêm để dùng .ToList()
using System.Threading.Tasks;

namespace Nhom16_MVC.Services
{
    public class AuthService
    {
        private readonly DatabaseService _dbService;
        private readonly EmailHelper _emailHelper;
        private readonly JwtHelper _jwtHelper;

        public AuthService(DatabaseService dbService, EmailHelper emailHelper, JwtHelper jwtHelper)
        {
            _dbService = dbService;
            _emailHelper = emailHelper;
            _jwtHelper = jwtHelper;
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var checkEmailQuery = "SELECT COUNT(*) FROM nguoidung WHERE email = @email";
                using (var checkCmd = new NpgsqlCommand(checkEmailQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@email", request.Email.ToLower());
                    var emailExists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

                    if (emailExists)
                    {
                        return new RegisterResponse
                        {
                            Success = false,
                            Message = "Email đã được sử dụng!"
                        };
                    }
                }

                var checkPhoneQuery = "SELECT COUNT(*) FROM nguoidung WHERE sodienthoai = @phone";
                using (var checkCmd = new NpgsqlCommand(checkPhoneQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@phone", request.SoDienThoai);
                    var phoneExists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

                    if (phoneExists)
                    {
                        return new RegisterResponse
                        {
                            Success = false,
                            Message = "Số điện thoại đã được sử dụng!"
                        };
                    }
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.MatKhau);
                var verificationToken = Guid.NewGuid().ToString();
                var tokenExpiry = DateTime.UtcNow.AddHours(24);

                var insertQuery = @"
                    INSERT INTO nguoidung 
                    (hoten, email, sodienthoai, matkhau, vaitro, sodutaikhoan, createdat, 
                     isemailverified, verificationtoken, tokenexpiry)
                    VALUES 
                    (@hoten, @email, @phone, @password, @vaitro, 0, NOW(), 
                     false, @token, @expiry)
                    RETURNING manguoidung, hoten, email, vaitro";

                using var insertCmd = new NpgsqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@hoten", request.HoTen);
                insertCmd.Parameters.AddWithValue("@email", request.Email.ToLower());
                insertCmd.Parameters.AddWithValue("@phone", request.SoDienThoai);
                insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                insertCmd.Parameters.AddWithValue("@vaitro", request.VaiTro);
                insertCmd.Parameters.AddWithValue("@token", verificationToken);
                insertCmd.Parameters.AddWithValue("@expiry", tokenExpiry);

                using var reader = await insertCmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var maNguoiDung = reader.GetInt32(0);
                    var hoTen = reader.GetString(1);
                    var email = reader.GetString(2);
                    var vaiTro = reader.GetString(3);

                    await reader.CloseAsync();

                    var emailSent = await _emailHelper.SendVerificationEmailAsync(
                        email,
                        hoTen,
                        verificationToken
                    );

                    return new RegisterResponse
                    {
                        Success = true,
                        Message = emailSent
                            ? "Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản."
                            : "Đăng ký thành công! Tuy nhiên không thể gửi email xác thực. Vui lòng liên hệ admin.",
                        Data = new
                        {
                            MaNguoiDung = maNguoiDung,
                            HoTen = hoTen,
                            Email = email,
                            VaiTro = vaiTro,
                            EmailSent = emailSent
                        }
                    };
                }

                return new RegisterResponse
                {
                    Success = false,
                    Message = "Đăng ký thất bại! Vui lòng thử lại."
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public async Task<RegisterResponse> VerifyEmailAsync(string token)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var findQuery = @"
                    SELECT manguoidung, hoten, email, tokenexpiry, isemailverified
                    FROM nguoidung 
                    WHERE verificationtoken = @token";

                using var findCmd = new NpgsqlCommand(findQuery, conn);
                findCmd.Parameters.AddWithValue("@token", token);

                using var reader = await findCmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Token không hợp lệ!"
                    };
                }

                var maNguoiDung = reader.GetInt32(0);
                var hoTen = reader.GetString(1);
                var email = reader.GetString(2);
                var tokenExpiry = reader.GetDateTime(3);
                var isEmailVerified = reader.GetBoolean(4);

                await reader.CloseAsync();

                if (isEmailVerified)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Email đã được xác thực trước đó!"
                    };
                }

                if (DateTime.UtcNow > tokenExpiry)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Token đã hết hạn! Vui lòng đăng ký lại."
                    };
                }

                var updateQuery = @"
                    UPDATE nguoidung 
                    SET isemailverified = true, 
                        verificationtoken = NULL, 
                        tokenexpiry = NULL
                    WHERE manguoidung = @id";

                using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@id", maNguoiDung);
                await updateCmd.ExecuteNonQueryAsync();

                await _emailHelper.SendWelcomeEmailAsync(email, hoTen);

                return new RegisterResponse
                {
                    Success = true,
                    Message = "Xác thực email thành công! Bạn có thể đăng nhập ngay bây giờ.",
                    Data = new
                    {
                        Email = email,
                        HoTen = hoTen
                    }
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public async Task<RegisterResponse> ResendVerificationEmailAsync(string email)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var findQuery = @"
                    SELECT manguoidung, hoten, isemailverified, verificationtoken
                    FROM nguoidung 
                    WHERE email = @email";

                using var findCmd = new NpgsqlCommand(findQuery, conn);
                findCmd.Parameters.AddWithValue("@email", email.ToLower());

                using var reader = await findCmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Email không tồn tại trong hệ thống!"
                    };
                }

                var maNguoiDung = reader.GetInt32(0);
                var hoTen = reader.GetString(1);
                var isEmailVerified = reader.GetBoolean(2);
                var oldToken = reader.IsDBNull(3) ? null : reader.GetString(3);

                await reader.CloseAsync();

                if (isEmailVerified)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Email đã được xác thực rồi!"
                    };
                }

                var newToken = Guid.NewGuid().ToString();
                var newExpiry = DateTime.UtcNow.AddHours(24);

                var updateQuery = @"
                    UPDATE nguoidung 
                    SET verificationtoken = @token, 
                        tokenexpiry = @expiry
                    WHERE manguoidung = @id";

                using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@token", newToken);
                updateCmd.Parameters.AddWithValue("@expiry", newExpiry);
                updateCmd.Parameters.AddWithValue("@id", maNguoiDung);
                await updateCmd.ExecuteNonQueryAsync();

                var emailSent = await _emailHelper.SendVerificationEmailAsync(email, hoTen, newToken);

                return new RegisterResponse
                {
                    Success = emailSent,
                    Message = emailSent
                        ? "Email xác thực đã được gửi lại! Vui lòng kiểm tra hộp thư."
                        : "Không thể gửi email! Vui lòng thử lại sau."
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                string query = @"
                    SELECT manguoidung, hoten, email, sodienthoai, avatar, matkhau, 
                        vaitro, sodutaikhoan, isemailverified, trangthai
                    FROM nguoidung 
                    WHERE email = @loginId OR sodienthoai = @loginId";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@loginId", request.LoginId.ToLower());

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Email/Số điện thoại hoặc mật khẩu không đúng!"
                    };
                }

                var maNguoiDung = reader.GetInt32(0);
                var hoTen = reader.GetString(1);
                var email = reader.GetString(2);
                var soDienThoai = reader.IsDBNull(3) ? null : reader.GetString(3);
                var avatar = reader.IsDBNull(4) ? null : reader.GetString(4);
                var hashedPassword = reader.GetString(5);
                var vaiTro = reader.GetString(6);
                var soDuTaiKhoan = reader.GetInt64(7);
                var isEmailVerified = reader.GetBoolean(8);
                string trangThai = reader.IsDBNull(9) ? "hoat_dong" : reader.GetString(9);

                await reader.CloseAsync();

                if (!isEmailVerified)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tài khoản chưa xác thực email! Vui lòng kiểm tra hộp thư."
                    };
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.MatKhau, hashedPassword);

                if (!isPasswordValid)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Email/Số điện thoại hoặc mật khẩu không đúng!"
                    };
                }

                if (trangThai == "bi_khoa")
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tài khoản của bạn đã bị khóa do vi phạm quy định. Vui lòng liên hệ Admin!"
                    };
                }

                var token = _jwtHelper.GenerateToken(maNguoiDung, email, vaiTro);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Đăng nhập thành công!",
                    Token = token,
                    User = new UserInfo
                    {
                        MaNguoiDung = maNguoiDung,
                        HoTen = hoTen,
                        Email = email,
                        SoDienThoai = soDienThoai,
                        Avatar = avatar,
                        VaiTro = vaiTro,
                        SoDuTaiKhoan = soDuTaiKhoan,
                        IsEmailVerified = isEmailVerified
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var checkUserQuery = "SELECT manguoidung, hoten, email FROM nguoidung WHERE email = @email LIMIT 1";
                string userName = string.Empty;
                string userEmail = string.Empty;
                bool userExists = false;

                using (var cmd = new NpgsqlCommand(checkUserQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@email", request.Email.ToLower().Trim());
                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        userExists = true;
                        userName = reader["hoten"]?.ToString() ?? "Thành viên SportSync";
                        userEmail = reader["email"].ToString();
                    }
                }

                if (!userExists)
                {
                    return new ForgotPasswordResponse { Success = false, Message = "Email này không tồn tại trên hệ thống!" };
                }

                string resetToken = Guid.NewGuid().ToString();
                DateTime expiryTime = DateTime.UtcNow.AddMinutes(5);

                var updateTokenQuery = @"
                    UPDATE nguoidung 
                    SET reset_token = @token, reset_token_expiry = @expiry 
                    WHERE email = @email";

                using (var updateCmd = new NpgsqlCommand(updateTokenQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@token", resetToken);
                    updateCmd.Parameters.AddWithValue("@expiry", expiryTime);
                    updateCmd.Parameters.AddWithValue("@email", userEmail);
                    await updateCmd.ExecuteNonQueryAsync();
                }

                bool isMailSent = await _emailHelper.SendResetPasswordEmailAsync(userEmail, userName, resetToken);

                if (!isMailSent)
                {
                    return new ForgotPasswordResponse { Success = false, Message = "Không thể gửi email lúc này. Vui lòng thử lại sau!" };
                }

                return new ForgotPasswordResponse { Success = true, Message = "Liên kết đặt lại mật khẩu đã được gửi vào email của bạn!" };
            }
            catch (Exception ex)
            {
                return new ForgotPasswordResponse { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" };
            }
        }
        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var checkTokenQuery = @"
                    SELECT email, reset_token_expiry FROM nguoidung 
                    WHERE reset_token = @token LIMIT 1";

                string dbEmail = string.Empty;
                DateTime? expiryTime = null;
                bool isTokenValid = false;

                using (var cmd = new NpgsqlCommand(checkTokenQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@token", request.Token.Trim());
                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        isTokenValid = true;
                        dbEmail = reader["email"]?.ToString();
                        expiryTime = reader.IsDBNull(1) ? null : (DateTime?)reader.GetDateTime(1);
                    }
                }

                if (!isTokenValid)
                {
                    return new ResetPasswordResponse { Success = false, Message = "Liên kết xác thực không chính xác!" };
                }

                if (string.IsNullOrEmpty(dbEmail) || !dbEmail.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return new ResetPasswordResponse { Success = false, Message = "Thông tin xác thực tài khoản không trùng khớp!" };
                }

                if (expiryTime.HasValue)
                {
                    var expiryUtc = DateTime.SpecifyKind(expiryTime.Value, DateTimeKind.Utc);
                    if (expiryUtc < DateTime.UtcNow)
                    {
                        return new ResetPasswordResponse { Success = false, Message = "Liên kết khôi phục mật khẩu này đã hết hạn (quá 5 phút)!" };
                    }
                }

                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                var updatePasswordQuery = @"
                    UPDATE nguoidung 
                    SET matkhau = @newPassword 
                    WHERE email = @email";

                using (var updateCmd = new NpgsqlCommand(updatePasswordQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@newPassword", hashedNewPassword);
                    updateCmd.Parameters.AddWithValue("@email", dbEmail);
                    await updateCmd.ExecuteNonQueryAsync();
                }

                return new ResetPasswordResponse { Success = true, Message = "Thay đổi mật khẩu thành công!" };
            }
            catch (Exception ex)
            {
                return new ResetPasswordResponse { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" };
            }
        }
        public async Task<List<StadiumApprovalViewDto>> GetUnapprovedStadiumsAsync()
        {
            var stadiumMap = new Dictionary<long, StadiumApprovalViewDto>();
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var query = @"
                    SELECT 
                        s.masanbong, s.tensan, s.diachi, s.mota, s.hinhanh, s.daduyet, 
                        n.hoten, m.link
                    FROM sanbong s
                    JOIN nguoidung n ON s.chusan = n.manguoidung
                    LEFT JOIN media_sanbong m ON s.masanbong = m.masanbong
                    WHERE s.daduyet = FALSE 
                    ORDER BY s.masanbong DESC";

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    long maSan = reader.GetInt64(reader.GetOrdinal("masanbong"));

                    if (!stadiumMap.ContainsKey(maSan))
                    {
                        stadiumMap[maSan] = new StadiumApprovalViewDto
                        {
                            MaSanBong = (int)maSan,
                            TenSan = reader["tensan"]?.ToString() ?? "",
                            ChuSan = reader["hoten"]?.ToString() ?? "Chủ sân",
                            DiaChi = reader["diachi"]?.ToString() ?? "",
                            MoTa = reader["mota"]?.ToString() ?? "",
                            HinhAnhDaiDien = reader["hinhanh"]?.ToString() ?? "",
                            DaDuyet = false,
                            DanhSachHinhAnhChiTiet = new List<string>()
                        };
                    }

                    int linkOrdinal = reader.GetOrdinal("link");
                    if (!reader.IsDBNull(linkOrdinal))
                    {
                        var linkAnh = reader.GetString(linkOrdinal);
                        if (!string.IsNullOrEmpty(linkAnh))
                        {
                            stadiumMap[maSan].DanhSachHinhAnhChiTiet.Add(linkAnh);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI GET SÂN CHƯA DUYỆT]: {ex.Message}");
            }
            return stadiumMap.Values.ToList();
        }
        public async Task<StadiumApprovalResponse> ProcessStadiumApprovalAsync(ApproveStadiumRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                var getStadiumQuery = @"
                    SELECT s.tensan, n.hoten, n.email 
                    FROM sanbong s
                    JOIN nguoidung n ON s.chusan = n.manguoidung
                    WHERE s.masanbong = @id LIMIT 1";

                string tenSan = "", tenChuSan = "", emailChuSan = "";
                bool isExist = false;

                using (var cmd = new NpgsqlCommand(getStadiumQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        isExist = true;
                        tenSan = reader["tensan"]?.ToString() ?? "Sân bóng";
                        tenChuSan = reader["hoten"]?.ToString() ?? "Đối tác";
                        emailChuSan = reader["email"]?.ToString() ?? "";
                    }
                }

                if (!isExist)
                {
                    return new StadiumApprovalResponse { Success = false, Message = "Sân bóng cần duyệt không tồn tại trên hệ thống!" };
                }

                if (request.IsApproved)
                {
                    var approveQuery = "UPDATE sanbong SET daduyet = TRUE WHERE masanbong = @id";
                    using var approveCmd = new NpgsqlCommand(approveQuery, conn);
                    approveCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    await approveCmd.ExecuteNonQueryAsync();
                }
                else
                {
                    if (string.IsNullOrEmpty(request.LyDoTuChoi))
                    {
                        return new StadiumApprovalResponse { Success = false, Message = "Vui lòng nhập lý do từ chối kiểm duyệt sân!" };
                    }

                    var deleteMediaQuery = "DELETE FROM media_sanbong WHERE masanbong = @id";
                    using var deleteMediaCmd = new NpgsqlCommand(deleteMediaQuery, conn);
                    deleteMediaCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    await deleteMediaCmd.ExecuteNonQueryAsync();

                    var deleteStadiumQuery = "DELETE FROM sanbong WHERE masanbong = @id";
                    using var deleteStadiumCmd = new NpgsqlCommand(deleteStadiumQuery, conn);
                    deleteStadiumCmd.Parameters.AddWithValue("@id", request.MaSanBong);
                    await deleteStadiumCmd.ExecuteNonQueryAsync();
                }

                // SỬA TẠI ĐÂY: Gọi thông qua _emailHelper của class thay vì gọi cục bộ
                if (!string.IsNullOrEmpty(emailChuSan))
                {
                    _ = _emailHelper.SendStadiumApprovalResultEmailAsync(emailChuSan, tenChuSan, tenSan, request.IsApproved, request.IsApproved ? "" : request.LyDoTuChoi);
                }

                return new StadiumApprovalResponse
                {
                    Success = true,
                    Message = request.IsApproved
                        ? "Đã phê duyệt sân bóng thành công và gửi email thông báo cho chủ sân!"
                        : "Đã từ chối, loại bỏ yêu cầu đăng ký sân và gửi email lý do cho chủ sân!"
                };
            }
            catch (Exception ex)
            {
                return new StadiumApprovalResponse { Success = false, Message = $"Lỗi xử lý kiểm duyệt: {ex.Message}" };
            }
        }
        public async Task<object> GetPendingWithdrawalsAsync()
        {
            var list = new List<object>();
            string query = @"
                SELECT y.mayeucau, y.manguoidung, y.sotien, y.trangthai, y.tennganhang, y.sotaikhoan, n.hoten, n.email
                FROM yeucauruttien y
                JOIN nguoidung n ON y.manguoidung = n.manguoidung
                WHERE y.trangthai = 'cho_xu_ly'
                ORDER BY y.mayeucau DESC";

            try
            {
                // Thay thế việc dùng _configuration bằng _dbService của nhóm bạn
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Đọc theo tên cột chuẩn xác, tránh lỗi index vị trí
                        list.Add(new
                        {
                            MaYeuCau = Convert.ToInt32(reader["mayeucau"]),
                            MaNguoiDung = Convert.ToInt32(reader["manguoidung"]),
                            SoTien = Convert.ToDecimal(reader["sotien"]),
                            TrangThai = reader["trangthai"]?.ToString() ?? "",
                            TenNganHang = reader["tennganhang"]?.ToString() ?? "",
                            SoTaiKhoan = reader["sotaikhoan"]?.ToString() ?? "",
                            HoTenNguoiRut = reader["hoten"]?.ToString() ?? "",
                            EmailNguoiRut = reader["email"]?.ToString() ?? ""
                        });
                    }
                }
                return new { Success = true, Data = list };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = "Lỗi lấy danh sách: " + ex.Message };
            }
        }
        public async Task<WithdrawalResponse> ProcessWithdrawalAsync(ProcessWithdrawalDto dto)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 1. Lấy thông tin tài khoản người rút để kiểm tra trạng thái và chuẩn bị gửi mail
                string checkQuery = @"
            SELECT y.trangthai, n.email, n.hoten, y.sotien 
            FROM yeucauruttien y 
            JOIN nguoidung n ON y.manguoidung = n.manguoidung 
            WHERE y.mayeucau = @MaYeuCau";

                string currentStatus = "";
                string userEmail = "";
                string userName = "";
                decimal soTienRut = 0;

                using (var cmdCheck = new NpgsqlCommand(checkQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@MaYeuCau", dto.MaYeuCau);
                    using (var reader = await cmdCheck.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            currentStatus = reader["trangthai"]?.ToString() ?? "";
                            userEmail = reader["email"]?.ToString() ?? "";
                            userName = reader["hoten"]?.ToString() ?? "";
                            soTienRut = Convert.ToDecimal(reader["sotien"]);
                        }
                        else
                        {
                            return new WithdrawalResponse { Success = false, Message = "Không tìm thấy yêu cầu rút tiền này." };
                        }
                    }
                }

                // Kiểm tra logic tầng nghiệp vụ
                if (currentStatus != "cho_xu_ly")
                {
                    return new WithdrawalResponse { Success = false, Message = $"Yêu cầu này đã được xử lý từ trước! (Trạng thái hiện tại: {currentStatus})." };
                }

                // 2. Tiến hành cập nhật trạng thái (ĐÃ FIX: Thay đổi 'lydotuchoi' thành cột 'mota' để khớp DB)
                string updateQuery = @"
            UPDATE yeucauruttien 
            SET trangthai = @TrangThaiMoi, mota = @LyDoTuChoi 
            WHERE mayeucau = @MaYeuCau";

                using (var cmdUpdate = new NpgsqlCommand(updateQuery, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@TrangThaiMoi", dto.TrangThaiMoi);
                    // Nếu admin từ chối thì lưu lý do vào cột mota, nếu duyệt thành công thì giữ nguyên mô tả cũ hoặc để trống
                    cmdUpdate.Parameters.AddWithValue("@LyDoTuChoi", (object)dto.LyDoTuChoi ?? DBNull.Value);
                    cmdUpdate.Parameters.AddWithValue("@MaYeuCau", dto.MaYeuCau);

                    await cmdUpdate.ExecuteNonQueryAsync();

                    // 3. --- LOGIC GỬI EMAIL TỰ ĐỘNG BẰNG EMAILHELPER CỦA NHÓM BẠN ---
                    string subject = dto.TrangThaiMoi == "da_chuyen" ? "Thông báo: Rút tiền thành công" : "Thông báo: Yêu cầu rút tiền bị từ chối";
                    string content = dto.TrangThaiMoi == "da_chuyen"
                        ? $"Chào {userName},\nYêu cầu rút số tiền {soTienRut:N0} VNĐ về tài khoản ngân hàng của bạn đã được Admin phê duyệt thành công!"
                        : $"Chào {userName},\nYêu cầu rút số tiền {soTienRut:N0} VNĐ của bạn đã bị từ chối.\nLý do: {dto.LyDoTuChoi}";

                    // Kích hoạt hàm gửi mail thực tế của nhóm bạn (Bỏ comment nếu EmailHelper đã sẵn sàng)
                    // await _emailHelper.SendEmailAsync(userEmail, subject, content);

                    return new WithdrawalResponse
                    {
                        Success = true,
                        Message = dto.TrangThaiMoi == "da_chuyen" ? "Đã phê duyệt và chuyển tiền thành công!" : "Đã từ chối yêu cầu rút tiền!"
                    };
                }
            }
            // --- BẮT CÁC LỖI RÀNG BUỘC PHÁT SINH TỪ TRIGGER POSTGRES ---
            catch (PostgresException ex)
            {
                if (ex.MessageText.Contains("ERROR_INVALID_STATUS"))
                {
                    return new WithdrawalResponse { Success = false, Message = "Thao tác thất bại: Yêu cầu rút tiền này đã được cập nhật trạng thái trước đó rồi!" };
                }
                return new WithdrawalResponse { Success = false, Message = "Lỗi xử lý cơ sở dữ liệu: " + ex.MessageText };
            }
            catch (Exception ex)
            {
                return new WithdrawalResponse { Success = false, Message = "Lỗi hệ thống: " + ex.Message };
            }
        }
        public async Task<object> GetAllUsersExceptAdminAsync(int currentAdminId)
        {
            var list = new List<object>();
            // Câu lệnh SQL lấy tất cả người dùng có vai trò không phải là Admin
            // Hoặc nếu là Admin thì loại trừ chính ID của Admin đang đăng nhập hiện tại
            string query = @"
        SELECT manguoidung, hoten, email, sodienthoai, vaitro, sodutaikhoan, trangthai, createdat
        FROM nguoidung
        WHERE vaitro <> 'Admin' OR manguoidung <> @currentAdminId
        ORDER BY manguoidung DESC";

            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@currentAdminId", currentAdminId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new
                            {
                                MaNguoiDung = Convert.ToInt32(reader["manguoidung"]),
                                HoTen = reader["hoten"]?.ToString() ?? "",
                                Email = reader["email"]?.ToString() ?? "",
                                SoDienThoai = reader["sodienthoai"]?.ToString() ?? "",
                                VaiTro = reader["vaitro"]?.ToString() ?? "",
                                SoDuTaiKhoan = Convert.ToInt64(reader["sodutaikhoan"]),
                                TrangThai = reader["trangthai"]?.ToString() ?? "hoat_dong",
                                NgayTao = Convert.ToDateTime(reader["createdat"])
                            });
                        }
                    }
                }
                return new { Success = true, Data = list };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = "Lỗi lấy danh sách tài khoản: " + ex.Message };
            }
        }
        public async Task<WithdrawalResponse> UpdateUserStatusAsync(UpdateUserStatusDto dto)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 1. Kiểm tra tài khoản cần khóa xem có tồn tại không
                string checkQuery = "SELECT vaitro FROM nguoidung WHERE manguoidung = @id";
                using (var cmdCheck = new NpgsqlCommand(checkQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@id", dto.MaNguoiDung);
                    using var reader = await cmdCheck.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                    {
                        return new WithdrawalResponse { Success = false, Message = "Không tìm thấy người dùng này trên hệ thống." };
                    }

                    string vaiTro = reader["vaitro"]?.ToString();
                    if (vaiTro?.ToLower() == "admin")
                    {
                        return new WithdrawalResponse { Success = false, Message = "Không thể thực hiện thao tác khóa trên tài khoản Quản trị viên khác!" };
                    }
                }

                // 2. Thực hiện cập nhật trạng thái mới vào CSDL
                string updateQuery = "UPDATE nguoidung SET trangthai = @trangThai WHERE manguoidung = @id";
                using (var cmdUpdate = new NpgsqlCommand(updateQuery, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@trangThai", dto.TrangThaiMoi);
                    cmdUpdate.Parameters.AddWithValue("@id", dto.MaNguoiDung);
                    await cmdUpdate.ExecuteNonQueryAsync();
                }

                string hanhDong = dto.TrangThaiMoi == "bi_khoa" ? "khóa" : "mở khóa";
                return new WithdrawalResponse { Success = true, Message = $"Đã {hanhDong} tài khoản thành công!" };
            }
            catch (Exception ex)
            {
                return new WithdrawalResponse { Success = false, Message = "Lỗi hệ thống khi cập nhật trạng thái: " + ex.Message };
            }
        }
        // 1. LẤY DANH SÁCH ĐẶT SÂN: Đẩy các đơn có sự cố lên đầu để Admin dễ xử lý
        public async Task<object> AdminGetAllBookingsAsync()
        {
            var bookings = new List<object>();
            string query = @"
        SELECT 
            ct.machitietdatsan, d.madatsan, nt.manguoidung AS ma_nguoi_thue, nt.hoten AS ten_nguoi_thue, nt.sodienthoai,
            sb.tensanchitiet, s.tensan, ct.giobatdau, ct.giokethuc,
            ct.trangthaidatsan, ct.covande, d.ngaydat, ct.giatien
        FROM public.chitietdatsan ct
        JOIN public.datsan d ON ct.madatsan = d.madatsan
        JOIN public.nguoidung nt ON d.nguoidhthue = nt.manguoidung
        JOIN public.sanbongchitiet sb ON ct.masanchitiet = sb.masanchitiet
        JOIN public.sanbong s ON sb.masanbong = s.masanbong
        ORDER BY ct.covande DESC, d.ngaydat DESC, ct.machitietdatsan DESC"; // Ưu tiên hiển thị sự cố trước

            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    bookings.Add(new
                    {
                        MaChiTietDatSan = Convert.ToInt32(reader["machitietdatsan"]),
                        MaDatSan = Convert.ToInt32(reader["madatsan"]),
                        MaNguoiThue = Convert.ToInt32(reader["ma_nguoi_thue"]),
                        TenNguoiThue = reader["ten_nguoi_thue"]?.ToString(),
                        SoDienThoai = reader["sodienthoai"]?.ToString(),
                        TenSanBong = reader["tensan"]?.ToString() + " - " + reader["tensanchitiet"]?.ToString(),
                        GioBatDau = Convert.ToDateTime(reader["giobatdau"]),
                        GioKetThuc = Convert.ToDateTime(reader["giokethuc"]),
                        TrangThaiDatSan = reader["trangthaidatsan"]?.ToString(),
                        CoVanDe = Convert.ToBoolean(reader["covande"]),
                        NgayDat = Convert.ToDateTime(reader["ngaydat"]),
                        GiaTien = Convert.ToInt64(reader["giatien"])
                    });
                }
                return new { Success = true, Data = bookings };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = "Lỗi khi lấy danh sách đặt sân: " + ex.Message };
            }
        }

        // 2. XỬ LÝ SỰ CỐ ĐẶT SÂN: Thêm tự động hoàn tiền vào ví khách khi đơn bị hủy
        public async Task<WithdrawalResponse> AdminResolveBookingIssueAsync(ResolveBookingIssueDto dto)
        {
            using var conn = _dbService.GetConnection();
            await conn.OpenAsync();

            // Sử dụng Transaction để đảm bảo cập nhật trạng thái và hoàn tiền phải đi liền với nhau
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                // Bước A: Lấy thông tin trạng thái cũ, số tiền đơn đặt và ID người thuê
                string infoQuery = @"
            SELECT ct.trangthaidatsan, ct.giatien, d.nguoidhthue 
            FROM public.chitietdatsan ct
            JOIN public.datsan d ON ct.madatsan = d.madatsan
            WHERE ct.machitietdatsan = @maChiTiet";

                string trangThaiCu = "";
                long giaTien = 0;
                int maKhachHang = 0;

                using (var cmdInfo = new NpgsqlCommand(infoQuery, conn, transaction))
                {
                    cmdInfo.Parameters.AddWithValue("@maChiTiet", dto.MaChiTietDatSan);
                    using var reader = await cmdInfo.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        trangThaiCu = reader["trangthaidatsan"]?.ToString();
                        giaTien = Convert.ToInt64(reader["giatien"]);
                        maKhachHang = Convert.ToInt32(reader["nguoidhthue"]);
                    }
                    else
                    {
                        return new WithdrawalResponse { Success = false, Message = "Không tìm thấy chi tiết đơn đặt sân." };
                    }
                }

                // Bước B: Cập nhật trạng thái sự cố đơn đặt sân
                string updateQuery = @"
            UPDATE public.chitietdatsan 
            SET trangthaidatsan = @trangThai, covande = @coVanDe 
            WHERE machitietdatsan = @maChiTiet";

                using (var cmdUpdate = new NpgsqlCommand(updateQuery, conn, transaction))
                {
                    cmdUpdate.Parameters.AddWithValue("@trangThai", dto.TrangThaiMoi);
                    cmdUpdate.Parameters.AddWithValue("@coVanDe", dto.CoVanDe);
                    cmdUpdate.Parameters.AddWithValue("@maChiTiet", dto.MaChiTietDatSan);
                    await cmdUpdate.ExecuteNonQueryAsync();
                }

                // Bước C: Nếu trạng thái đổi sang 'da_huy' và đơn trước đó chưa hủy -> Tiến hành hoàn tiền cho khách
                if (dto.TrangThaiMoi == "da_huy" && trangThaiCu != "da_huy")
                {
                    string refundQuery = @"
                UPDATE public.nguoidung 
                SET sodutaikhoan = sodutaikhoan + @soTienHoan 
                WHERE manguoidung = @maKhach";

                    using var cmdRefund = new NpgsqlCommand(refundQuery, conn, transaction);
                    cmdRefund.Parameters.AddWithValue("@soTienHoan", giaTien);
                    cmdRefund.Parameters.AddWithValue("@maKhach", maKhachHang);
                    await cmdRefund.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                string message = dto.TrangThaiMoi == "da_huy"
                    ? "Đã hủy đơn đặt sân do sự cố và hoàn trả tiền vào ví của khách hàng thành công!"
                    : "Cập nhật trạng thái xử lý đơn đặt sân thành công!";

                return new WithdrawalResponse { Success = true, Message = message };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new WithdrawalResponse { Success = false, Message = "Lỗi hệ thống khi xử lý đơn đặt sân: " + ex.Message };
            }
        }

        // 3. LẤY DANH SÁCH ĐÁNH GIÁ: Sửa chuẩn tên cột theo Database thực tế
        public async Task<object> AdminGetAllRatingsAsync()
        {
            var ratings = new List<object>();
            string query = @"
        SELECT 
            dg.madanhgia, dg.manguoidung, nd.hoten AS ten_nguoi_dung,
            dg.masanbong, s.tensan AS ten_san_bong,
            dg.sosao, dg.noidungdanhgia, dg.thoigiandanhgia
        FROM public.danhgia dg
        JOIN public.nguoidung nd ON dg.manguoidung = nd.manguoidung
        JOIN public.sanbong s ON dg.masanbong = s.masanbong
        ORDER BY dg.thoigiandanhgia DESC";

            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ratings.Add(new
                    {
                        MaDanhGia = Convert.ToInt32(reader["madanhgia"]),
                        MaNguoiDung = Convert.ToInt32(reader["manguoidung"]),
                        TenNguoiDung = reader["ten_nguoi_dung"]?.ToString(),
                        MaSanBong = Convert.ToInt32(reader["masanbong"]),
                        TenSanBong = reader["ten_san_bong"]?.ToString(),
                        SoSao = Convert.ToInt32(reader["sosao"]),
                        NoiDungDanhGia = reader["noidungdanhgia"]?.ToString(),
                        ThoiGianDanhGia = Convert.ToDateTime(reader["thoigiandanhgia"])
                    });
                }
                return new { Success = true, Data = ratings };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = "Lỗi khi lấy danh sách đánh giá: " + ex.Message };
            }
        }

        // 4. XÓA ĐÁNH GIÁ: Sửa lỗi trả về sai kiểu dữ liệu ở block catch
        public async Task<WithdrawalResponse> AdminDeleteRatingAsync(int maDanhGia)
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
                    return new WithdrawalResponse { Success = false, Message = "Không tìm thấy mã đánh giá này hoặc đánh giá đã bị xóa trước đó." };
                }

                return new WithdrawalResponse { Success = true, Message = "Đã gỡ bỏ đánh giá không phù hợp thành công! " };
            }
            catch (Exception ex)
            {
                // Đã sửa từ đối tượng ẩn danh sang định dạng chuẩn WithdrawalResponse
                return new WithdrawalResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống khi xóa đánh giá: " + ex.Message
                };
            }
        }
    }
}