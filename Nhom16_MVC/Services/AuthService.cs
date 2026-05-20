using Nhom16_MVC.Models;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Helpers;
using Npgsql;
using BCrypt.Net;
using System;
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

                var query = @"
                    SELECT manguoidung, hoten, email, sodienthoai, avatar, matkhau, 
                           vaitro, sodutaikhoan, isemailverified
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
        // ====================================================================
        // TÍNH NĂNG: YÊU CẦU QUÊN MẬT KHẨU (HIỆU LỰC 5 PHÚT)
        // ====================================================================
        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // Kiểm tra email y hệt cách hệ thống đang làm ở hàm Login
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
                DateTime expiryTime = DateTime.UtcNow.AddMinutes(5); // Hiệu lực chuẩn 5 phút

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

        // ====================================================================
        // TÍNH NĂNG: ĐẶT LẠI MẬT KHẨU (BẤM NHIỀU LẦN THOẢI MÁI TRONG 5 PHÚT)
        // ====================================================================
        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // Tìm kiếm trực tiếp bằng Token để loại bỏ lỗi không khớp chuỗi/múi giờ của Postgres
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

                // 1. Kiểm tra xem Token có tồn tại trong DB không
                if (!isTokenValid)
                {
                    return new ResetPasswordResponse { Success = false, Message = "Liên kết xác thực không chính xác!" };
                }

                // 2. So khớp email bằng C# để đảm bảo tính an toàn tuyệt đối
                if (string.IsNullOrEmpty(dbEmail) || !dbEmail.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return new ResetPasswordResponse { Success = false, Message = "Thông tin xác thực tài khoản không trùng khớp!" };
                }

                // 3. Kiểm tra thời hạn 5 phút (Chuyển về UTC đồng bộ)
                if (expiryTime.HasValue)
                {
                    var expiryUtc = DateTime.SpecifyKind(expiryTime.Value, DateTimeKind.Utc);
                    if (expiryUtc < DateTime.UtcNow)
                    {
                        return new ResetPasswordResponse { Success = false, Message = "Liên kết khôi phục mật khẩu này đã hết hạn (quá 5 phút)!" };
                    }
                }

                // 4. Mã hóa mật khẩu mới bằng BCrypt và cập nhật lại vào DB
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
    }
}