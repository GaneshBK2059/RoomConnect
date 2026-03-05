using System.Data;
using Npgsql;
using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Database;

namespace RoomConnect.Infrastructure.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly IDbConnectionFactory _factory;

        public RefreshTokenRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<RefreshToken?> GetByTokenStrAsync(string tokenHash)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT * FROM refresh_tokens WHERE token_hash = @TokenHash LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@TokenHash", tokenHash);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new RefreshToken
            {
                Id = (long)reader["id"],
                UserId = (long)reader["user_id"],
                TokenHash = reader["token_hash"].ToString()!,
                ExpiresAt = (DateTime)reader["expires_at"],
                CreatedAt = (DateTime)reader["created_at"],
                RevokedAt = reader["revoked_at"] as DateTime?
            };
        }

        public async Task<long> CreateAsync(RefreshToken refreshToken)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO refresh_tokens (user_id, token_hash, expires_at, created_at, revoked_at)
                VALUES (@UserId, @TokenHash, @ExpiresAt, @CreatedAt, @RevokedAt)
                RETURNING id;", conn);

            cmd.Parameters.AddWithValue("@UserId", refreshToken.UserId);
            cmd.Parameters.AddWithValue("@TokenHash", refreshToken.TokenHash);
            cmd.Parameters.AddWithValue("@ExpiresAt", refreshToken.ExpiresAt);
            cmd.Parameters.AddWithValue("@CreatedAt", refreshToken.CreatedAt);
            cmd.Parameters.AddWithValue("@RevokedAt", (object?)refreshToken.RevokedAt ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return (long)result!;
        }

        public async Task<bool> RevokeAsync(long id, DateTime revokedAt)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "UPDATE refresh_tokens SET revoked_at = @RevokedAt WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@RevokedAt", revokedAt);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> RevokeAllForUserAsync(long userId, DateTime revokedAt)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "UPDATE refresh_tokens SET revoked_at = @RevokedAt WHERE user_id = @UserId AND revoked_at IS NULL", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RevokedAt", revokedAt);

            await cmd.ExecuteNonQueryAsync();
            return true;
        }
    }
}
