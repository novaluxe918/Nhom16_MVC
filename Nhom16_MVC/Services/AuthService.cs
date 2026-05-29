using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> RegisterAsync(AuthDTOs model);
        Task<ServiceResponse<LoginResultDto>> VerifyEmailAsync(VerifyEmailDto model);
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
        public string Token { get; set; } = string.Empty;
    }

    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? ErrorDetail { get; set; }
        public bool EmailSent { get; set; } = false;

        // Sửa lỗi: Thêm định nghĩa thuộc tính này để Controller không bị lỗi biên dịch
        public bool RequiresEmailVerification { get; set; } = false;
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly EmailHelper _emailHelper;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, EmailHelper emailHelper, JwtHelper jwtHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
            _jwtHelper = jwtHelper;
        }

        public async Task<ServiceResponse<int>> RegisterAsync(AuthDTOs model)
        {
            try
            {
                // 1. Kiểm tra trùng lặp Email hoặc Số điện thoại
                var isExist = await _context.nguoidung.AnyAsync(u => u.email == model.Email || u.sodienthoai == model.SoDienThoai);
                if (isExist)
                {
                    return new ServiceResponse<int>
                    {
                        Success = false,
                        Message = "Email hoặc số điện thoại này đã được sử dụng trong hệ thống!"
                    };
                }

                // 2. Ép kiểu an toàn từ chuỗi String sang VaiTroEnum
                if (!Enum.TryParse<VaiTroEnum>(model.VaiTro, true, out var parsedRole))
                {
                    parsedRole = VaiTroEnum.nguoiThue;
                }

                // 3. Tạo mã OTP ngẫu nhiên
                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();

                // 4. Khởi tạo đối tượng thực thể (Đồng bộ DateTime.Now tránh lệch múi giờ Postgres)
                var user = new nguoidung
                {
                    hoten = model.HoTen,
                    email = model.Email,
                    sodienthoai = model.SoDienThoai,
                    matkhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    vaitro = parsedRole,
                    sodutaikhoan = 0,
                    isemailverified = false,
                    verificationtoken = otp,
                    tokenexpiry = DateTime.Now.AddMinutes(15),
                    trangthai = "hoat_dong",
                    createdat = DateTime.Now
                };

                _context.nguoidung.Add(user);
                await _context.SaveChangesAsync();

                // 5. Gửi Email OTP kích hoạt tài khoản
                bool emailSent = false;
                try
                {
                    string emailBody = GetOtpEmailTemplate(model.HoTen, otp, "Cảm ơn bạn đã đăng ký tài khoản tại SportSync. Vui lòng sử dụng mã OTP dưới đây để hoàn tất xác thực tài khoản.");
                    await _emailHelper.SendEmailAsync(model.Email, "Xác Thực Đăng Ký Tài Khoản - SportSync", emailBody);
                    emailSent = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                }

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Đăng ký tài khoản thành công! Vui lòng kiểm tra email để lấy mã OTP xác thực.",
                    Data = user.manguoidung,
                    EmailSent = emailSent
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Xảy ra lỗi hệ thống trong quá trình đăng ký tài khoản.",
                    ErrorDetail = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<ServiceResponse<LoginResultDto>> VerifyEmailAsync(VerifyEmailDto model)
        {
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email);
                if (user == null)
                    return new ServiceResponse<LoginResultDto> { Success = false, Message = "Tài khoản không tồn tại." };

                if (user.isemailverified == true)
                    return new ServiceResponse<LoginResultDto> { Success = false, Message = "Tài khoản này đã được xác thực từ trước." };

                // Sửa model.Otp thành model.OTP cho đúng thuộc tính AuthDTOs.cs
                if (user.verificationtoken != model.OTP || user.tokenexpiry < DateTime.Now)
                    return new ServiceResponse<LoginResultDto> { Success = false, Message = "Mã xác thực không chính xác hoặc đã hết hạn." };

                user.isemailverified = true;
                user.verificationtoken = null;
                user.tokenexpiry = null;

                await _context.SaveChangesAsync();

                // Đã an toàn nhờ hàm nạp chồng (overload) 2 tham số trong JwtHelper
                string token = _jwtHelper.GenerateToken(user, user.vaitro.ToString());

                return new ServiceResponse<LoginResultDto>
                {
                    Success = true,
                    Message = "Xác thực email thành công! Đã tự động đăng nhập vào hệ thống.",
                    Data = new LoginResultDto
                    {
                        UserId = user.manguoidung,
                        Email = user.email,
                        HoTen = user.hoten,
                        VaiTro = user.vaitro.ToString(),
                        Token = token
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResultDto> { Success = false, Message = "Lỗi hệ thống khi xác thực.", ErrorDetail = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> ResendEmailOtpAsync(ForgotPasswordDto model)
        {
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email);
                if (user == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Không tìm thấy tài khoản với email này." };

                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();

                user.verificationtoken = otp;
                user.tokenexpiry = DateTime.Now.AddMinutes(15);

                await _context.SaveChangesAsync();

                bool emailSent = false;
                try
                {
                    string emailBody = GetOtpEmailTemplate(user.hoten, otp, "Hệ thống đã tạo một mã OTP mới theo yêu cầu của bạn. Vui lòng nhập mã này để xác thực.");
                    await _emailHelper.SendEmailAsync(user.email, "Gửi Lại Mã Xác Thực OTP - SportSync", emailBody);
                    emailSent = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi lại email: {ex.Message}");
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Mã OTP mới đã được gửi vào email của bạn.",
                    Data = true,
                    EmailSent = emailSent
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Lỗi hệ thống khi gửi lại mã OTP.", ErrorDetail = ex.Message };
            }
        }

        public async Task<ServiceResponse<LoginResultDto>> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.MatKhau, user.matkhau))
                    return new ServiceResponse<LoginResultDto> { Success = false, Message = "Email hoặc mật khẩu không chính xác." };

                if (user.isemailverified != true)
                {
                    return new ServiceResponse<LoginResultDto>
                    {
                        Success = false,
                        RequiresEmailVerification = true, // Gắn cờ true để controller nhận diện
                        Message = "Tài khoản của bạn chưa được xác thực email. Vui lòng xác thực trước khi đăng nhập!"
                    };
                }

                if (user.trangthai == "bi_khoa")
                    return new ServiceResponse<LoginResultDto> { Success = false, Message = "Tài khoản của bạn hiện đang bị khóa." };

                // Đã an toàn nhờ hàm nạp chồng (overload) 2 tham số trong JwtHelper
                string token = _jwtHelper.GenerateToken(user, user.vaitro.ToString());

                return new ServiceResponse<LoginResultDto>
                {
                    Success = true,
                    Message = "Đăng nhập thành công!",
                    Data = new LoginResultDto
                    {
                        UserId = user.manguoidung,
                        Email = user.email,
                        HoTen = user.hoten,
                        VaiTro = user.vaitro.ToString(),
                        Token = token
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResultDto> { Success = false, Message = "Lỗi hệ thống khi đăng nhập.", ErrorDetail = ex.Message };
            }
        }

        public async Task<ServiceResponse<int>> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email);
                if (user == null)
                    return new ServiceResponse<int> { Success = false, Message = "Email không tồn tại trong hệ thống." };

                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();

                user.verificationtoken = otp;
                user.tokenexpiry = DateTime.Now.AddMinutes(15);

                await _context.SaveChangesAsync();

                bool emailSent = false;
                try
                {
                    string emailBody = GetOtpEmailTemplate(user.hoten, otp, "Bạn đã yêu cầu khôi phục mật khẩu. Vui lòng sử dụng mã OTP dưới đây để thiết lập lại mật khẩu mới.");
                    await _emailHelper.SendEmailAsync(user.email, "Yêu Cầu Khôi Phục Mật Khẩu - SportSync", emailBody);
                    emailSent = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi mail quên mật khẩu: {ex.Message}");
                }

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Mã OTP khôi phục mật khẩu đã được gửi qua email.",
                    Data = user.manguoidung,
                    EmailSent = emailSent
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int> { Success = false, Message = "Lỗi hệ thống xử lý quên mật khẩu.", ErrorDetail = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto model)
        {
            try
            {
                var user = await _context.nguoidung.FirstOrDefaultAsync(u => u.email == model.Email);
                if (user == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Tài khoản không tồn tại." };

                if (user.verificationtoken != model.OTP || user.tokenexpiry < DateTime.Now)
                    return new ServiceResponse<bool> { Success = false, Message = "Mã OTP khôi phục không chính xác hoặc đã hết hạn." };

                // ĐÃ FIX LỖI: Chuyển đổi hoàn toàn sang model.MatKhauMoi cho khớp với AuthDTOs.cs
                user.matkhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
                user.verificationtoken = null;
                user.tokenexpiry = null;

                await _context.SaveChangesAsync();

                return new ServiceResponse<bool> { Success = true, Message = "Đặt lại mật khẩu thành công! Hãy đăng nhập lại bằng mật khẩu mới.", Data = true };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Lỗi hệ thống khi đặt lại mật khẩu.", ErrorDetail = ex.Message };
            }
        }

        private string GetOtpEmailTemplate(string name, string otp, string bodyText)
        {
            return $@"
    <html>
        <head>
            <style>
                .email-container {{ font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }}
                .email-header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                .email-body {{ padding: 24px; background-color: #ffffff; color: #333333; line-height: 1.6; }}
                .welcome-text {{ font-size: 18px; margin-top: 0; }}
                .main-desc {{ font-size: 15px; color: #555555; margin-bottom: 20px; }}
                .otp-box {{ background-color: #f4fbf5; border: 2px dashed #4CAF50; border-radius: 6px; padding: 20px; text-align: center; margin: 24px 0; }}
                .otp-title {{ font-size: 14px; color: #666666; text-transform: uppercase; letter-spacing: 1px; margin: 0 0 10px 0; }}
                .otp-code {{ font-size: 36px; font-weight: bold; color: #4CAF50; letter-spacing: 6px; margin: 0; }}
                .warning-text {{ font-size: 13px; color: #e53935; margin: 10px 0 0 0; font-weight: 500; }}
                .divider {{ border: 0; border-top: 1px solid #eeeeee; margin: 24px 0; }}
                .email-footer {{ background-color: #f9f9f9; padding: 20px; text-align: center; font-size: 12px; color: #888888; border-top: 1px solid #e0e0e0; }}
                .footer-text {{ margin: 0 0 12px 0; line-height: 1.5; }}
                .footer-links a {{ color: #4CAF50; text-decoration: none; margin: 0 8px; }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>
                    <h2>HỆ THỐNG SPORTSYNC</h2>
                </div>
                <div class='email-body'>
                    <p class='welcome-text'>Xin chào <strong>{name}</strong>,</p>
                    <p class='main-desc'>{bodyText}</p>
                    
                    <div class='otp-box'>
                        <p class='otp-title'>Mã xác thực của bạn</p>
                        <h1 class='otp-code'>{otp}</h1>
                        <p class='warning-text'>⏱️ Mã này có hiệu lực trong vòng 15 phút</p>
                    </div>
                    
                    <p class='main-desc' style='margin-bottom: 0;'>Vì lý do bảo mật, vui lòng tuyệt đối không chia sẻ mã này cho bất kỳ ai khác.</p>
                    <hr class='divider' />
                </div>
                <div class='email-footer'>
                    <p class='footer-text'>
                        Đây là email tự động từ hệ thống quản lý sân bóng SportSync.<br>
                        Nếu bạn không thực hiện yêu cầu này, bạn có thể an tâm bỏ qua email này.<br>
                    </p>
                    <div class='footer-links'>
                        <a href='#'>Trang chủ</a> • 
                        <a href='#'>Hỗ trợ kỹ thuật</a> • 
                        <a href='#'>Điều khoản dịch vụ</a>
                    </div>
                </div>
            </div>
        </body>
    </html>";
        }
    }
}