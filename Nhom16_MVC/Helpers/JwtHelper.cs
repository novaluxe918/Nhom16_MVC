using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nhom16_MVC.Models.Entities;

namespace Nhom16_MVC.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo Token với 1 tham số là đối tượng nguoidung (Lấy vai trò trực tiếp từ object)
        /// </summary>
        public string GenerateToken(nguoidung user)
        {
            return GenerateTokenInternal(user, user.vaitro.ToString());
        }

        /// <summary>
        /// Overload: Tạo Token với 2 tham số (Sửa lỗi: No overload for method 'GenerateToken' takes 2 arguments)
        /// </summary>
        public string GenerateToken(nguoidung user, string customRole)
        {
            return GenerateTokenInternal(user, customRole);
        }

        /// <summary>
        /// Hàm xử lý logic tạo Token nội bộ
        /// </summary>
        private string GenerateTokenInternal(nguoidung user, string role)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryInHours = double.Parse(_configuration["JwtSettings:ExpiryInHours"] ?? "24");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.manguoidung.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, role),
                new Claim("HoTen", user.hoten ?? "")
            };

            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(expiryInHours),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(token);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}