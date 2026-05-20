using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Nhom16_MVC.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendVerificationEmailAsync(string toEmail, string userName, string verificationToken)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var appSettings = _configuration.GetSection("AppSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    emailSettings["SenderName"],
                    emailSettings["SenderEmail"]
                ));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Xác thực tài khoản SportSync";

                // Tạo link xác thực
                var verificationLink = $"{appSettings["BaseUrl"]}/api/auth/verify-email?token={verificationToken}";

                // HTML email template
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                          color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                                .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                                .button {{ display: inline-block; padding: 15px 30px; background: #667eea; 
                                          color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                                .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>🏟️ SportSync</h1>
                                    <p>Hệ thống Quản lý Đặt Sân Bóng</p>
                                </div>
                                <div class='content'>
                                    <h2>Xin chào {userName}!</h2>
                                    <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>SportSync</strong>.</p>
                                    <p>Vui lòng nhấn vào nút bên dưới để xác thực email của bạn:</p>
                                    <div style='text-align: center;'>
                                        <a href='{verificationLink}' class='button'>
                                            ✅ Xác Thực Email
                                        </a>
                                    </div>
                                    <p>Hoặc copy link sau vào trình duyệt:</p>
                                    <p style='background: #fff; padding: 10px; border-radius: 5px; word-break: break-all;'>
                                        {verificationLink}
                                    </p>
                                    <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau 24 giờ.</p>
                                    <p>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.</p>
                                </div>
                                <div class='footer'>
                                    <p>© 2026 SportSync - Hệ thống Quản lý Sân Bóng</p>
                                    <p>Email này được gửi tự động, vui lòng không reply.</p>
                                </div>
                            </div>
                        </body>
                        </html>
                    "
                };

                message.Body = bodyBuilder.ToMessageBody();

                // Gửi email
                using var client = new SmtpClient();
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    emailSettings["SenderEmail"],
                    emailSettings["SenderPassword"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    emailSettings["SenderName"],
                    emailSettings["SenderEmail"]
                ));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Chào mừng đến với SportSync!";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2>🎉 Chào mừng {userName}!</h2>
                                <p>Tài khoản của bạn đã được xác thực thành công!</p>
                                <p>Bây giờ bạn có thể đăng nhập và sử dụng đầy đủ các tính năng của SportSync.</p>
                                <p><strong>Bắt đầu ngay:</strong></p>
                                <ul>
                                    <li>Tìm kiếm sân bóng gần bạn</li>
                                    <li>Đặt sân trực tuyến nhanh chóng</li>
                                    <li>Quản lý lịch đặt sân</li>
                                    <li>Đánh giá và review sân</li>
                                </ul>
                                <p>Chúc bạn có trải nghiệm tuyệt vời! ⚽</p>
                                <hr>
                                <p style='color: #666; font-size: 12px;'>© 2026 SportSync</p>
                            </div>
                        </body>
                        </html>
                    "
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    emailSettings["SenderEmail"],
                    emailSettings["SenderPassword"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email chào mừng: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> SendResetPasswordEmailAsync(string toEmail, string userName, string resetToken)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var appSettings = _configuration.GetSection("AppSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    emailSettings["SenderName"] ?? "SportSync Support",
                    emailSettings["SenderEmail"]
                ));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Đặt lại mật khẩu tài khoản SportSync";

                var resetLink = $"{appSettings["BaseUrl"]}/api/auth/reset-password?token={resetToken}&email={Uri.EscapeDataString(toEmail)}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='utf-8'>
                            <style>
                                body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; color: #222222; background-color: #f9fafb; padding: 20px; }}
                                .wrapper {{ max-width: 480px; margin: 0 auto; background: #ffffff; padding: 24px; border-radius: 8px; border: 1px solid #e5e7eb; }}
                                h2 {{ color: #10b981; font-size: 18px; font-weight: 600; margin-top: 0; margin-bottom: 16px; }}
                                .btn-container {{ text-align: center; margin: 24px 0; }}
                                .button {{ display: inline-block; padding: 11px 22px; background-color: #10b981; color: #ffffff !important; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; }}
                                .notice {{ color: #6b7280; font-size: 13px; background: #f3f4f6; padding: 12px; border-radius: 6px; margin-top: 20px; }}
                                .footer {{ margin-top: 28px; font-size: 12px; color: #9ca3af; border-top: 1px solid #f3f4f6; padding-top: 14px; }}
                            </style>
                        </head>
                        <body>
                            <div class='wrapper'>
                                <h2>Đặt lại mật khẩu của bạn</h2>
                                <p>Chào {userName},</p>
                                <p>Chúng tôi nhận được yêu cầu thay đổi mật khẩu cho tài khoản SportSync của bạn. Bạn hãy bấm vào nút bên dưới để tiến hành thiết lập mật khẩu mới:</p>
                                
                                <div class='btn-container'>
                                    <a href='{resetLink}' class='button'>Đổi mật khẩu mới</a>
                                </div>
                                
                                <p>Yêu cầu này sẽ hết hạn sau <strong>5 phút</strong>.</p>
                                
                                <div class='notice'>
                                    Nếu bạn không thực hiện yêu cầu này, bạn có thể an tâm bỏ qua email. Mật khẩu hiện tại của bạn vẫn được giữ an toàn.
                                </div>
                                
                                <div class='footer'>
                                    Đội ngũ hỗ trợ SportSync
                                </div>
                            </div>
                        </body>
                        </html>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"] ?? "587"),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    emailSettings["SenderEmail"],
                    emailSettings["SenderPassword"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error]: {ex.Message}");
                return false;
            }
        }
    }
}