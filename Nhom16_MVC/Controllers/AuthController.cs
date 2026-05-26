using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;
using System;
using System.Threading.Tasks;

namespace Nhom16_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailHelper _emailHelper;

        public AuthController(AppDbContext context, EmailHelper emailHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
        }

        /// <summary>
        /// 1. Đăng ký tài khoản mới - Gửi OTP xác thực email
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthDTOs model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu đầu vào không hợp lệ!"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState
                    });
                }

                var userExists = await _context.nguoidung.AnyAsync(u => u.email == model.Email.ToLower());
                if (userExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email này đã được đăng ký!"
                    });
                }

                if (!string.IsNullOrEmpty(model.SoDienThoai))
                {
                    var phoneExists = await _context.nguoidung.AnyAsync(u => u.sodienthoai == model.SoDienThoai);
                    if (phoneExists)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Số điện thoại này đã được đăng ký!"
                        });
                    }
                }

                // Tạo OTP 6 chữ số
                string otp = new Random().Next(100000, 999999).ToString();

                var newUser = new nguoidung
                {
                    hoten = model.HoTen.Trim(),
                    email = model.Email.ToLower().Trim(),
                    sodienthoai = model.SoDienThoai?.Trim(),
                    matkhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    vaitro = model.VaiTro,
                    isemailverified = false,
                    verificationtoken = otp,
                    tokenexpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(15), DateTimeKind.Unspecified),
                    sodutaikhoan = 0
                };

                _context.nguoidung.Add(newUser);
                await _context.SaveChangesAsync();

                try
                {
                    await _emailHelper.SendEmailAsync(
                        newUser.email,
                        "Xác Thực Email - SportSync",
                        $@"
<html>
    <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
        <div style='background-color: white; max-width: 600px; margin: 0 auto; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
            <h2 style='color: #007bff; text-align: center;'>Xác Thực Email Tài Khoản</h2>
            <hr style='border: none; border-top: 2px solid #007bff;'>
            <p>Xin chào <strong>{newUser.hoten}</strong>,</p>
            <p>Cảm ơn bạn đã đăng ký tài khoản SportSync!</p>
            <p>Mã OTP xác thực email của bạn là:</p>
            <div style='text-align: center; padding: 20px;'>
                <h1 style='color: #007bff; letter-spacing: 5px; font-size: 32px; margin: 0;'>{otp}</h1>
            </div>
            <p style='text-align: center; color: #666;'><strong>⏰ Mã này có hiệu lực trong 15 phút</strong></p>
            <p>Vui lòng nhập mã này vào ứng dụng để xác thực email của bạn.</p>
            <hr style='border: none; border-top: 1px solid #ddd;'>
            <p style='color: #999; font-size: 12px; text-align: center;'>
                Nếu bạn không yêu cầu đăng ký, vui lòng bỏ qua email này.
            </p>
        </div>
    </body>
</html>"
                    );
                }
                catch
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Đăng ký thành công! Tuy nhiên gửi email thất bại. Vui lòng kiểm tra email hoặc yêu cầu gửi lại OTP.",
                        userId = newUser.manguoidung,
                        emailSent = false
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Đăng ký thành công! OTP xác thực đã được gửi đến email của bạn.",
                    userId = newUser.manguoidung,
                    emailSent = true
                });
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = dbEx.Message;
                if (dbEx.InnerException != null)
                {
                    errorMessage += " | Inner: " + dbEx.InnerException.Message;
                }

                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi cơ sở dữ liệu khi đăng ký",
                    error = errorMessage,
                    exceptionType = "DbUpdateException"
                });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                }

                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi đăng ký",
                    error = errorMessage
                });
            }
        }

        /// <summary>
        /// 2. Xác thực email bằng OTP
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ!"
                    });
                }

                var user = await _context.nguoidung.FirstOrDefaultAsync(u =>
                    u.email == model.Email.ToLower().Trim());

                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không tồn tại!"
                    });
                }

                if (user.isemailverified == true)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email này đã được xác thực rồi!"
                    });
                }

                if (user.verificationtoken != model.OTP)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "OTP không chính xác!"
                    });
                }

                if (user.tokenexpiry == null || user.tokenexpiry < DateTime.UtcNow)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "OTP đã hết hạn! Vui lòng yêu cầu gửi lại OTP."
                    });
                }

                user.isemailverified = true;
                user.verificationtoken = null;
                user.tokenexpiry = null;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Xác thực email thành công! Bây giờ bạn có thể đăng nhập.",
                    userId = user.manguoidung
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi xác thực email",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 3. Gửi lại OTP xác thực email
        /// </summary>
        [HttpPost("resend-email-otp")]
        public async Task<IActionResult> ResendEmailOtp([FromBody] ForgotPasswordDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không hợp lệ!"
                    });
                }

                var user = await _context.nguoidung.FirstOrDefaultAsync(u =>
                    u.email == model.Email.ToLower().Trim());

                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không tồn tại!"
                    });
                }

                if (user.isemailverified == true)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email này đã được xác thực rồi!"
                    });
                }

                string otp = new Random().Next(100000, 999999).ToString();
                user.verificationtoken = otp;
                user.tokenexpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(15), DateTimeKind.Unspecified);

                await _context.SaveChangesAsync();

                try
                {
                    await _emailHelper.SendEmailAsync(
                        user.email,
                        "Gửi Lại OTP Xác Thực - SportSync",
                        $@"
<html>
    <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
        <div style='background-color: white; max-width: 600px; margin: 0 auto; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
            <h2 style='color: #007bff; text-align: center;'>Mã OTP Xác Thực Email</h2>
            <hr style='border: none; border-top: 2px solid #007bff;'>
            <p>Xin chào <strong>{user.hoten}</strong>,</p>
            <p>Dưới đây là mã OTP mới để xác thực email của bạn:</p>
            <div style='text-align: center; padding: 20px;'>
                <h1 style='color: #007bff; letter-spacing: 5px; font-size: 32px; margin: 0;'>{otp}</h1>
            </div>
            <p style='text-align: center; color: #666;'><strong>⏰ Mã này có hiệu lực trong 15 phút</strong></p>
            <hr style='border: none; border-top: 1px solid #ddd;'>
            <p style='color: #999; font-size: 12px; text-align: center;'>
                Nếu bạn không yêu cầu, vui lòng bỏ qua email này.
            </p>
        </div>
    </body>
</html>"
                    );
                }
                catch
                {
                    return Ok(new
                    {
                        success = true,
                        message = "OTP mới đã được tạo nhưng gửi email thất bại!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "OTP mới đã được gửi đến email của bạn!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi gửi lại OTP",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 4. Đăng nhập - Phải xác thực email trước
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email và mật khẩu không được để trống!"
                    });
                }

                var user = await _context.nguoidung.FirstOrDefaultAsync(u =>
                    u.email == model.Email.ToLower().Trim());

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.MatKhau, user.matkhau))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Tài khoản hoặc mật khẩu không chính xác!"
                    });
                }

                // ✅ Kiểm tra email đã xác thực
                if (user.isemailverified != true)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email chưa được xác thực! Vui lòng kiểm tra email và nhập OTP.",
                        requiresEmailVerification = true,
                        userId = user.manguoidung
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công!",
                    userId = user.manguoidung,
                    email = user.email,
                    hoTen = user.hoten,
                    vaiTro = user.vaitro.ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi đăng nhập",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 5. Quên mật khẩu - Bước 1: Nhập email → Gửi OTP
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không hợp lệ!"
                    });
                }

                var user = await _context.nguoidung.FirstOrDefaultAsync(u =>
                    u.email == model.Email.ToLower().Trim());

                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không tồn tại!"
                    });
                }

                // Tạo OTP 6 chữ số
                string otp = new Random().Next(100000, 999999).ToString();

                user.verificationtoken = otp;
                user.tokenexpiry = DateTime.UtcNow.AddMinutes(15);

                await _context.SaveChangesAsync();

                try
                {
                    await _emailHelper.SendEmailAsync(
                        user.email,
                        "Mã OTP Khôi Phục Mật Khẩu - SportSync",
                        $@"
<html>
    <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
        <div style='background-color: white; max-width: 600px; margin: 0 auto; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
            <h2 style='color: #ff6b6b; text-align: center;'>Khôi Phục Mật Khẩu</h2>
            <hr style='border: none; border-top: 2px solid #ff6b6b;'>
            <p>Xin chào <strong>{user.hoten}</strong>,</p>
            <p>Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn.</p>
            <p>Mã OTP để đặt lại mật khẩu của bạn là:</p>
            <div style='text-align: center; padding: 20px;'>
                <h1 style='color: #ff6b6b; letter-spacing: 5px; font-size: 32px; margin: 0;'>{otp}</h1>
            </div>
            <p style='text-align: center; color: #666;'><strong>⏰ Mã này có hiệu lực trong 15 phút</strong></p>
            <p>Vui lòng nhập mã này cùng với mật khẩu mới để khôi phục tài khoản.</p>
            <hr style='border: none; border-top: 1px solid #ddd;'>
            <p style='color: #999; font-size: 12px; text-align: center;'>
                Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này.
            </p>
        </div>
    </body>
</html>"
                    );
                }
                catch
                {
                    return Ok(new
                    {
                        success = true,
                        message = "OTP đã được tạo nhưng gửi email thất bại!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Mã OTP đã được gửi đến email của bạn! Vui lòng kiểm tra email.",
                    userId = user.manguoidung
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi gửi OTP",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 6. Đặt lại mật khẩu - Bước 2: Nhập Email + OTP + Mật khẩu mới
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ!"
                    });
                }

                var user = await _context.nguoidung.FirstOrDefaultAsync(u =>
                    u.email == model.Email.ToLower().Trim());

                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không tồn tại!"
                    });
                }

                // Kiểm tra OTP
                if (user.verificationtoken != model.OTP)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "OTP không chính xác!"
                    });
                }

                // Kiểm tra OTP hết hạn
                if (user.tokenexpiry == null || user.tokenexpiry < DateTime.UtcNow)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "OTP đã hết hạn! Vui lòng yêu cầu OTP mới."
                    });
                }

                // ✅ Cập nhật mật khẩu mới
                user.matkhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
                user.verificationtoken = null;
                user.tokenexpiry = null;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Đặt lại mật khẩu thành công! Bây giờ bạn có thể đăng nhập với mật khẩu mới."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi đặt lại mật khẩu",
                    error = ex.Message
                });
            }
        }
    }
}