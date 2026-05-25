using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Nhom16_MVC.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgreSQL");
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}