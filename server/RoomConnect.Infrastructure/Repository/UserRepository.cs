using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Database;
using Npgsql;
using System.Data;

namespace RoomConnect.Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly IDbConnectionFactory _factory;

        public UserRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read()) return null;

            return Map(reader);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"SELECT *
                  FROM users
                  WHERE email = @Email
                  LIMIT 1", 
                conn);

            cmd.Parameters.AddWithValue("@Email", email);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read()) return null;

            return Map(reader);
        }

        public async Task<long> CreateAsync(User user)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO users
                (full_name, email, phone, password_hash, role,
                 is_active, email_verified, avatar_url, bio, sys_created, sys_updated)
                VALUES
                (@FullName, @Email, @Phone, @PasswordHash, @Role,
                 @IsActive, @EmailVerified, @AvatarUrl, @Bio, NOW(), NOW())
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@EmailVerified", user.EmailVerified);
            cmd.Parameters.AddWithValue("@AvatarUrl", (object?)user.AvatarUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Bio", (object?)user.Bio ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();

            return (long)result!;
        }

        private User Map(IDataRecord r)
        {
            return new User
            {
                Id = (long)r["id"],
                FullName = r["full_name"].ToString()!,
                Email = r["email"].ToString()!,
                Phone = r["phone"] as string,
                PasswordHash = r["password_hash"].ToString()!,
                Role = r["role"].ToString()!,
                IsActive = (bool)r["is_active"],
                EmailVerified = (bool)r["email_verified"],
                AvatarUrl = r["avatar_url"] as string,
                Bio = r["bio"] as string,
                SysCreated = (DateTime)r["sys_created"],
                SysUpdated = (DateTime)r["sys_updated"]
            };
        }
    }
}
