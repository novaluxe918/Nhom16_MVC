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

        public AuthService(DatabaseService dbService, EmailHelper emailHelper)
        {
            _dbService = dbService;
            _emailHelper = emailHelper;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 1. Kiểm tra email đã tồn tại chưa
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

                // 2. Kiểm tra số điện thoại đã tồn tại chưa
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

                // 3. Hash mật khẩu
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.MatKhau);

                // 4. Tạo verification token (UUID)
                var verificationToken = Guid.NewGuid().ToString();
                var tokenExpiry = DateTime.UtcNow.AddHours(24); // Token hết hạn sau 24h

                // 5. Insert người dùng mới vào database
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

                    // 6. Gửi email xác thực
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

        /// <summary>
        /// Xác thực email bằng token
        /// </summary>
        public async Task<RegisterResponse> VerifyEmailAsync(string token)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 1. Tìm user với token này
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

                // 2. Kiểm tra đã verify chưa
                if (isEmailVerified)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Email đã được xác thực trước đó!"
                    };
                }

                // 3. Kiểm tra token hết hạn chưa
                if (DateTime.UtcNow > tokenExpiry)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Token đã hết hạn! Vui lòng đăng ký lại."
                    };
                }

                // 4. Cập nhật trạng thái verified
                var updateQuery = @"
                    UPDATE nguoidung 
                    SET isemailverified = true, 
                        verificationtoken = NULL, 
                        tokenexpiry = NULL
                    WHERE manguoidung = @id";

                using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@id", maNguoiDung);
                await updateCmd.ExecuteNonQueryAsync();

                // 5. Gửi email chào mừng
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

        /// <summary>
        /// Gửi lại email xác thực
        /// </summary>
        public async Task<RegisterResponse> ResendVerificationEmailAsync(string email)
        {
            try
            {
                using var conn = _dbService.GetConnection();
                await conn.OpenAsync();

                // 1. Tìm user với email
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

                // 2. Kiểm tra đã verify chưa
                if (isEmailVerified)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Email đã được xác thực rồi!"
                    };
                }

                // 3. Tạo token mới
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

                // 4. Gửi lại email
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
    }
}