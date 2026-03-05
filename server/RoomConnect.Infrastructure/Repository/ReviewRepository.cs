using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Database;
using Npgsql;
using System.Data;

namespace RoomConnect.Infrastructure.Repositories
{
    public class ReviewRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ReviewRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<Review>> GetByRoomIdAsync(Guid roomId)
        {
            var reviews = new List<Review>();
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM reviews WHERE room_id = @RoomId ORDER BY sys_created DESC", conn);
            cmd.Parameters.AddWithValue("@RoomId", roomId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.NextResultAsync())
            {
                reviews.Add(Map(reader));
            }

            return reviews;
        }

        public async Task<long> CreateAsync(Review review)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO reviews
                (room_id, user_id, rating, title, comment)
                VALUES
                (@RoomId, @UserId, @Rating, @Title, @Comment)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("@RoomId", review.RoomId);
            cmd.Parameters.AddWithValue("@UserId", review.UserId);
            cmd.Parameters.AddWithValue("@Rating", review.Rating);
            cmd.Parameters.AddWithValue("@Title", review.Title);
            cmd.Parameters.AddWithValue("@Comment", review.Comment);

            var result = await cmd.ExecuteScalarAsync();
            return (long)result!;
        }

        private Review Map(IDataRecord r)
        {
            return new Review
            {
                Id = (long)r["id"],
                RoomId = (Guid)r["room_id"],
                UserId = (long)r["user_id"],
                Rating = (int)r["rating"],
                Title = r["title"].ToString()!,
                Comment = r["comment"].ToString()!,
                SysCreated = (DateTime)r["sys_created"],
                SysUpdated = (DateTime)r["sys_updated"]
            };
        }
    }
}
