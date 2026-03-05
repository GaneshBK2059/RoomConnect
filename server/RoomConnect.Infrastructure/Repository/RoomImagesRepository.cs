using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Database;
using Npgsql;
using System.Data;

namespace RoomConnect.Infrastructure.Repositories
{
    public class RoomImagesRepository
    {
        private readonly IDbConnectionFactory _factory;

        public RoomImagesRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<RoomImage>> GetByRoomIdAsync(Guid roomId)
        {
            var images = new List<RoomImage>();
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM room_images WHERE room_id = @RoomId ORDER BY created_at ASC", conn);
            cmd.Parameters.AddWithValue("@RoomId", roomId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.NextResultAsync())
            {
                images.Add(Map(reader));
            }

            return images;
        }

        public async Task<long> CreateAsync(RoomImage image)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO room_images (room_id, image_url, created_at)
                VALUES (@RoomId, @ImageUrl, NOW())
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("@RoomId", image.RoomId);
            cmd.Parameters.AddWithValue("@ImageUrl", image.ImageUrl);

            var result = await cmd.ExecuteScalarAsync();
            return (long)result!;
        }

        public async Task DeleteAsync(long id)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("DELETE FROM room_images WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        private RoomImage Map(IDataRecord r)
        {
            return new RoomImage
            {
                Id = (long)r["id"],
                RoomId = (Guid)r["room_id"],
                ImageUrl = r["image_url"].ToString()!,
                CreatedAt = (DateTime)r["created_at"]
            };
        }
    }
}
