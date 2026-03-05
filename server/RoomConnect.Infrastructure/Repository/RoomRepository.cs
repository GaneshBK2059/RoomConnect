using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Database;
using Npgsql;
using System.Data;

namespace RoomConnect.Infrastructure.Repositories
{
    public class RoomRepository
    {
        private readonly IDbConnectionFactory _factory;

        public RoomRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<Room>> GetAllAsync()
        {
            var rooms = new List<Room>();
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM rooms WHERE is_available = true ORDER BY created_at DESC", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rooms.Add(Map(reader));
            }

            return rooms;
        }

        public async Task<Room?> GetByIdAsync(Guid id)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM rooms WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read()) return null;

            return Map(reader);
        }

        public async Task<List<Room>> GetByLandlordIdAsync(long landlordId)
        {
            var rooms = new List<Room>();
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM rooms WHERE landlord_id = @LandlordId ORDER BY created_at DESC", conn);
            cmd.Parameters.AddWithValue("@LandlordId", landlordId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rooms.Add(Map(reader));
            }

            return rooms;
        }

        public async Task<Guid> CreateAsync(Room room)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO rooms
                (landlord_id, title, description, address, city, state, zip_code, latitude, longitude,
                 price, room_type, max_guests, bedrooms, bathrooms, wifi, parking, ac, heating, image_url, is_available)
                VALUES
                (@LandlordId, @Title, @Description, @Address, @City, @State, @ZipCode, @Latitude, @Longitude,
                 @Price, @RoomType, @MaxGuests, @Bedrooms, @Bathrooms, @WiFi, @Parking, @AC, @Heating, @ImageUrl, @IsAvailable)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("@LandlordId", room.LandlordId);
            cmd.Parameters.AddWithValue("@Title", room.Title);
            cmd.Parameters.AddWithValue("@Description", room.Description);
            cmd.Parameters.AddWithValue("@Address", room.Address);
            cmd.Parameters.AddWithValue("@City", room.City);
            cmd.Parameters.AddWithValue("@State", room.State);
            cmd.Parameters.AddWithValue("@ZipCode", room.ZipCode);
            cmd.Parameters.AddWithValue("@Latitude", room.Latitude);
            cmd.Parameters.AddWithValue("@Longitude", room.Longitude);
            cmd.Parameters.AddWithValue("@Price", room.Price);
            cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
            cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
            cmd.Parameters.AddWithValue("@Bedrooms", room.Bedrooms);
            cmd.Parameters.AddWithValue("@Bathrooms", room.Bathrooms);
            cmd.Parameters.AddWithValue("@WiFi", room.WiFi);
            cmd.Parameters.AddWithValue("@Parking", room.Parking);
            cmd.Parameters.AddWithValue("@AC", room.AC);
            cmd.Parameters.AddWithValue("@Heating", room.Heating);
            cmd.Parameters.AddWithValue("@ImageUrl", room.ImageUrl);
            cmd.Parameters.AddWithValue("@IsAvailable", room.IsAvailable);

            var result = await cmd.ExecuteScalarAsync();
            return (Guid)result!;
        }

        public async Task UpdateAsync(Room room)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                UPDATE rooms SET
                title = @Title, description = @Description, address = @Address, city = @City,
                state = @State, zip_code = @ZipCode, latitude = @Latitude, longitude = @Longitude,
                price = @Price, room_type = @RoomType, max_guests = @MaxGuests, bedrooms = @Bedrooms,
                bathrooms = @Bathrooms, wifi = @WiFi, parking = @Parking, ac = @AC, heating = @Heating,
                image_url = @ImageUrl, is_available = @IsAvailable, updated_at = NOW()
                WHERE id = @Id AND landlord_id = @LandlordId
            ", conn);

            cmd.Parameters.AddWithValue("@Id", room.Id);
            cmd.Parameters.AddWithValue("@LandlordId", room.LandlordId);
            cmd.Parameters.AddWithValue("@Title", room.Title);
            cmd.Parameters.AddWithValue("@Description", room.Description);
            cmd.Parameters.AddWithValue("@Address", room.Address);
            cmd.Parameters.AddWithValue("@City", room.City);
            cmd.Parameters.AddWithValue("@State", room.State);
            cmd.Parameters.AddWithValue("@ZipCode", room.ZipCode);
            cmd.Parameters.AddWithValue("@Latitude", room.Latitude);
            cmd.Parameters.AddWithValue("@Longitude", room.Longitude);
            cmd.Parameters.AddWithValue("@Price", room.Price);
            cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
            cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
            cmd.Parameters.AddWithValue("@Bedrooms", room.Bedrooms);
            cmd.Parameters.AddWithValue("@Bathrooms", room.Bathrooms);
            cmd.Parameters.AddWithValue("@WiFi", room.WiFi);
            cmd.Parameters.AddWithValue("@Parking", room.Parking);
            cmd.Parameters.AddWithValue("@AC", room.AC);
            cmd.Parameters.AddWithValue("@Heating", room.Heating);
            cmd.Parameters.AddWithValue("@ImageUrl", room.ImageUrl);
            cmd.Parameters.AddWithValue("@IsAvailable", room.IsAvailable);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("DELETE FROM rooms WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        private Room Map(IDataRecord r)
        {
            return new Room
            {
                Id = (Guid)r["id"],
                LandlordId = (long)r["landlord_id"],
                Title = r["title"].ToString()!,
                Description = r["description"].ToString()!,
                Address = r["address"].ToString()!,
                City = r["city"].ToString()!,
                State = r["state"].ToString()!,
                ZipCode = r["zip_code"].ToString()!,
                Latitude = Convert.ToDecimal(r["latitude"]),
                Longitude = Convert.ToDecimal(r["longitude"]),
                Price = Convert.ToDecimal(r["price"]),
                RoomType = r["room_type"].ToString()!,
                MaxGuests = Convert.ToInt32(r["max_guests"]),
                Bedrooms = Convert.ToInt32(r["bedrooms"]),
                Bathrooms = Convert.ToInt32(r["bathrooms"]),
                WiFi = (bool)r["wifi"],
                Parking = (bool)r["parking"],
                AC = (bool)r["ac"],
                Heating = (bool)r["heating"],
                ImageUrl = r["image_url"].ToString() ?? "",
                IsAvailable = (bool)r["is_available"],
                AvgRating = r["avg_rating"] is DBNull ? 0 : Convert.ToDouble(r["avg_rating"]),
                ReviewCount = r["review_count"] is DBNull ? 0 : Convert.ToInt32(r["review_count"]),
                CreatedAt = (DateTime)r["created_at"],
                UpdatedAt = (DateTime)r["updated_at"]
            };
        }
    }
}
