using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Nhom16_MVC.Models;
using Nhom16_MVC.Models.DTOs;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class StadiumService
    {
        private readonly DatabaseService _db;

        public StadiumService(DatabaseService db)
        {
            _db = db;
        }

        public async Task<List<SanBong>> GetMySanBongsAsync(int chuSanId)
        {
            var sanBong = new List<SanBong>();
            using var connection = _db.GetConnection();
            await connection.OpenAsync();
            string sql = @"
                SELECT
                    maSanBong,
                    chuSan,
                    tenSan,
                    moTa,
                    hinhAnh,
                    daDuyet,
                    diaChi,
                    quan,
                    huyen,
                    xa,
                    thanhPho,
                    kinhDo,
                    viDo,
                    createdAt
                FROM SANBONG
                WHERE chuSan = @chuSan
                ORDER BY createdAt DESC
            ";
            using var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("chuSan", chuSanId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sanBong.Add(new SanBong()
                {
                    MaSanBong = reader.GetInt32(0),
                    ChuSan = reader.GetInt32(1),
                    TenSan = reader.GetString(2),
                    MoTa = reader.IsDBNull(3) ? null : reader.GetString(3),
                    HinhAnh = reader.IsDBNull(4) ? null : reader.GetString(4),
                    DaDuyet = reader.GetBoolean(5),
                    DiaChi = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Quan = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Huyen = reader.IsDBNull(8)
                        ? null
                        : reader.GetString(8),

                    Xa = reader.IsDBNull(9)
                        ? null
                        : reader.GetString(9),

                    ThanhPho = reader.IsDBNull(10)
                        ? null
                        : reader.GetString(10),

                    KinhDo = reader.IsDBNull(11)
                        ? null
                        : reader.GetDecimal(11),

                    ViDo = reader.IsDBNull(12)
                        ? null
                        : reader.GetDecimal(12),
                    CreatedAt = reader.GetDateTime(13)

                });
            }

            return sanBong;
        }

        public async Task CreateSanBongAsync(int chuSanId, CreateStadiumDto stadiumDto)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();
            string sql = @"
                INSERT INTO SANBONG
                (
                    chuSan,
                    tenSan,
                    moTa,
                    daDuyet,
                    diaChi,
                    quan,
                    huyen,
                    xa,
                    thanhPho,
                    kinhDo,
                    viDo
                )
                VALUES
                (
                    @chuSan,
                    @tenSan,
                    @moTa,
                    FALSE,
                    @diaChi,
                    @quan,
                    @huyen,
                    @xa,
                    @thanhPho,
                    @kinhDo,
                    @viDo
                )
            ";
            using var cmd = new Npgsql.NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("chuSan", chuSanId);
            cmd.Parameters.AddWithValue("tenSan", stadiumDto.TenSan);
            cmd.Parameters.AddWithValue("moTa", (object?)stadiumDto.MoTa ?? DBNull.Value);
            cmd.Parameters.AddWithValue("diaChi", (object?)stadiumDto.DiaChi ?? DBNull.Value);
            cmd.Parameters.AddWithValue(
    "quan",
    (object?)stadiumDto.Quan ?? DBNull.Value
);

            cmd.Parameters.AddWithValue(
                "huyen",
                (object?)stadiumDto.Huyen ?? DBNull.Value
            );

            cmd.Parameters.AddWithValue(
                "xa",
                (object?)stadiumDto.Xa ?? DBNull.Value
            );

            cmd.Parameters.AddWithValue(
                "thanhPho",
                (object?)stadiumDto.ThanhPho ?? DBNull.Value
            );

            cmd.Parameters.AddWithValue(
                "kinhDo",
                (object?)stadiumDto.KinhDo ?? DBNull.Value
            );

            cmd.Parameters.AddWithValue(
                "viDo",
                (object?)stadiumDto.ViDo ?? DBNull.Value
            );

            await cmd.ExecuteNonQueryAsync();
        }

    
    public async Task<int?> GetFirstChuSanIdAsync()
        {
            using var conn = _db.GetConnection();

            await conn.OpenAsync();

            string sql = @"
        SELECT maNguoiDung
        FROM NGUOIDUNG
        WHERE vaiTro = 'chuSan'
        LIMIT 1
    ";

            using var cmd = new NpgsqlCommand(sql, conn);

            var result = await cmd.ExecuteScalarAsync();

            if (result == null)
            {
                return null;
            }

            return Convert.ToInt32(result);
        }
    }

    }
