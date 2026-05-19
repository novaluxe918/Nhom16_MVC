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
    }
}