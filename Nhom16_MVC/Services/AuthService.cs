using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> RegisterAsync(AuthDTOs model);
        Task<ServiceResponse<int>> VerifyEmailAsync(VerifyEmailDto model);
        Task<ServiceResponse<bool>> ResendEmailOtpAsync(ForgotPasswordDto model);
        Task<ServiceResponse<LoginResultDto>> LoginAsync(LoginDto model);
        Task<ServiceResponse<int>> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto model);
    }

    public class LoginResultDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty; // Thêm trường Token vào DTO kết quả
    }

    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool EmailSent { get; set; } = true;
        public bool RequiresEmailVerification { get; set; } = false;
        public string? ErrorDetail { get; set; }
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly EmailHelper _emailHelper;
        private readonly JwtHelper _jwtHelper; // Khai báo thêm JwtHelper

        // Tiêm JwtHelper vào qua Constructor
        public AuthService(AppDbContext context, EmailHelper emailHelper, JwtHelper jwtHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
            _jwtHelper = jwtHelper;
        }

        public async Task<ServiceResponse<int>> RegisterAsync(AuthDTOs model)
        {
            var response = new ServiceResponse<int>();
            try
            {
                var userExists = await _context.nguoidung.AnyAsync(u => u.email == model.Email.ToLower().Trim());
                if (userExists)
                {
                    response.Success = false;
                    response.Message = "Email này đã được đăng ký!";
                    return response;
                }

                if (!string.IsNullOrEmpty(model.SoDienThoai))
                {
                    var phoneExists = await _context.nguoidung.AnyAsync(u => u.sodienthoai == model.SoDienThoai.Trim());
                    if (phoneExists)
                    {
                        response.Success = false;
                        response.Message = "Số điện thoại này đã được đăng ký!";
                        return response;
                    }
                }

                string otp = new Random().Next(100000, 999999).ToString();

                DateTime nowLocal = DateTime.Now;
                DateTime expiryTime = nowLocal.AddMinutes(15);

                var newUser = new nguoidung
                {
                    hoten = model.HoTen.Trim(),
                    email = model.Email.ToLower().Trim(),
                    sodienthoai = model.SoDienThoai?.Trim(),
                    matkhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    vaitro = model.VaiTro,
                    isemailverified = false,
                    verificationtoken = otp,
                    tokenexpiry = DateTime.SpecifyKind(expiryTime, DateTimeKind.Unspecified),
                    sodutaikhoan = 0,
                    createdat = DateTime.SpecifyKind(nowLocal, DateTimeKind.Unspecified)
                };

                _context.nguoidung.Add(newUser);
                await _context.SaveChangesAsync();

                response.Data = newUser.manguoidung;

                try
                {
                    await _emailHelper.SendEmailAsync(
                        newUser.email,
                        "Xác Thực Email - SportSync",
                        GetEmailHtmlTemplate(newUser.hoten, otp, "Xác Thực Email Tài Khoản", "Cảm ơn bạn đã đăng ký tài khoản SportSync! Mã OTP xác thực email của bạn là:", "#007bff")
                    );
                }
                catch
                {
                    response.EmailSent = false;
                    response.Message = "Đăng ký thành công! Tuy nhiên gửi email thất bại. Vui lòng yêu cầu gửi lại OTP.";
                    return response;
                }

                response.Message = "Đăng ký thành công! OTP xác thực đã được gửi đến email of bạn.";
            }
            catch (DbUpdateException dbEx)
            {
                response.Success = false;
                response.Message = "Lỗi cơ sở dữ liệu khi đăng ký tài khoản.";
                response.ErrorDetail = dbEx.InnerException?.Message ?? dbEx.Message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống khi đăng ký tài khoản.";
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<int>> VerifyEmailAsync(VerifyEmailDto model)
        {
            var response = new ServiceResponse<int>();
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email.ToLower().Trim());
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Email không tồn tại!";
                    return response;
                }

                if (user.isemailverified == true)
                {
                    response.Success = false;
                    response.Message = "Email này đã được xác thực rồi!";
                    return response;
                }

                if (user.verificationtoken != model.OTP.Trim())
                {
                    response.Success = false;
                    response.Message = "OTP không chính xác!";
                    return response;
                }

                if (user.tokenexpiry == null || user.tokenexpiry < DateTime.Now)
                {
                    response.Success = false;
                    response.Message = "OTP đã hết hạn! Vui lòng yêu cầu gửi lại OTP.";
                    return response;
                }

                user.isemailverified = true;
                user.verificationtoken = null;
                user.tokenexpiry = null;

                await _context.SaveChangesAsync();
                response.Data = user.manguoidung;
                response.Message = "Xác thực email thành công! Bây giờ bạn có thể đăng nhập.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống khi xác thực email.";
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> ResendEmailOtpAsync(ForgotPasswordDto model)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email.ToLower().Trim());
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Email không tồn tại!";
                    return response;
                }

                if (user.isemailverified == true)
                {
                    response.Success = false;
                    response.Message = "Email này đã được xác thực rồi!";
                    return response;
                }

                string otp = new Random().Next(100000, 999999).ToString();
                user.verificationtoken = otp;
                user.tokenexpiry = DateTime.SpecifyKind(DateTime.Now.AddMinutes(15), DateTimeKind.Unspecified);

                await _context.SaveChangesAsync();
                response.Data = true;

                try
                {
                    await _emailHelper.SendEmailAsync(
                        user.email,
                        "Gửi Lại OTP Xác Thực - SportSync",
                        GetEmailHtmlTemplate(user.hoten, otp, "Mã OTP Xác Thực Email", "Dưới đây là mã OTP mới để xác thực email của bạn:", "#007bff")
                    );
                }
                catch
                {
                    response.EmailSent = false;
                    response.Message = "OTP mới đã được tạo nhưng gửi email thất bại!";
                    return response;
                }

                response.Message = "OTP mới đã được gửi đến email của bạn!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống khi gửi lại OTP.";
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<LoginResultDto>> LoginAsync(LoginDto model)
        {
            var response = new ServiceResponse<LoginResultDto>();
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email.ToLower().Trim());
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.MatKhau, user.matkhau))
                {
                    response.Success = false;
                    response.Message = "Tài khoản hoặc mật khẩu không chính xác!";
                    return response;
                }

                if (user.isemailverified != true)
                {
                    response.Success = false;
                    response.RequiresEmailVerification = true;
                    response.Message = "Email chưa được xác thực! Vui lòng kiểm tra email và nhập OTP.";
                    response.Data = new LoginResultDto { UserId = user.manguoidung };
                    return response;
                }

                // 🌟 TẠO TOKEN JWT: Gọi hàm sinh token thông qua đối tượng user
                string generatedToken = _jwtHelper.GenerateToken(user);

                response.Data = new LoginResultDto
                {
                    UserId = user.manguoidung,
                    Email = user.email,
                    HoTen = user.hoten,
                    VaiTro = user.vaitro.ToString(),
                    Token = generatedToken // Trả kèm token về
                };
                response.Message = "Đăng nhập thành công!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống khi đăng nhập.";
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<int>> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var response = new ServiceResponse<int>();
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email.ToLower().Trim());
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Email không tồn tại!";
                    return response;
                }

                string otp = new Random().Next(100000, 999999).ToString();
                user.verificationtoken = otp;
                user.tokenexpiry = DateTime.SpecifyKind(DateTime.Now.AddMinutes(15), DateTimeKind.Unspecified);

                await _context.SaveChangesAsync();
                response.Data = user.manguoidung;

                try
                {
                    await _emailHelper.SendEmailAsync(
                        user.email,
                        "Mã OTP Khôi Phục Mật Khẩu - SportSync",
                        GetEmailHtmlTemplate(user.hoten, otp, "Khôi Phục Mật Khẩu", "Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn. Mã OTP của bạn là:", "#ff6b6b")
                    );
                }
                catch
                {
                    response.EmailSent = false;
                    response.Message = "OTP khôi phục đã được tạo nhưng gửi email thất bại!";
                    return response;
                }

                response.Message = "Mã OTP đã được gửi đến email của bạn! Vui lòng kiểm tra email.";
            }
            catch (DbUpdateException dbEx)
            {
                response.Success = false;
                response.Message = "Lỗi cơ sở dữ liệu khi yêu cầu khôi phục mật khẩu.";
                response.ErrorDetail = dbEx.InnerException?.Message ?? dbEx.Message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống khi yêu cầu khôi phục mật khẩu.";
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto model)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email.ToLower().Trim());
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Email không tồn tại!";
                    return response;
                }

                if (user.verificationtoken != model.OTP.Trim())
                {
                    response.Success = false;
                    response.Message = "OTP không chính xác!";
                    return response;
                }

                if (user.tokenexpiry == null || user.tokenexpiry < DateTime.Now)
                {
                    response.Success = false;
                    response.Message = "OTP đã hết hạn! Vui lòng yêu cầu OTP mới.";
                    return response;
                }

                user.matkhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
                user.verificationtoken = null;
                user.tokenexpiry = null;

                await _context.SaveChangesAsync();
                response.Data = true;
                response.Message = "Đặt lại mật khẩu thành công! Bạn đã có thể đăng nhập với mật khẩu mới.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống khi đặt lại mật khẩu.";
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        private string GetEmailHtmlTemplate(string name, string otp, string title, string bodyText, string themeColor)
        {
            return $@"
            <html>
                <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
                    <div style='background-color: white; max-width: 600px; margin: 0 auto; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color: {themeColor}; text-align: center;'>{title}</h2>
                        <hr style='border: none; border-top: 2px solid {themeColor};'>
                        <p>Xin chào <strong>{name}</strong>,</p>
                        <p>{bodyText}</p>
                        <div style='text-align: center; padding: 20px;'>
                            <h1 style='color: {themeColor}; letter-spacing: 5px; font-size: 32px; margin: 0;'>{otp}</h1>
                        </div>
                        <p style='text-align: center; color: #666;'><strong>⏰ Mã này có hiệu lực trong 15 phút</strong></p>
                        <hr style='border: none; border-top: 1px solid #ddd;'>
                        <p style='color: #999; font-size: 12px; text-align: center;'>
                            Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
                        </p>
                    </div>
                </body>
            </html>";
        }
    }
}