using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Database;
using Npgsql;
using System.Data;

namespace RoomConnect.Infrastructure.Repositories
{
    public class BookingRepository
    {
        private readonly IDbConnectionFactory _factory;

        public BookingRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<Booking>> GetByUserIdAsync(long userId)
        {
            var bookings = new List<Booking>();
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM bookings WHERE user_id = @UserId ORDER BY check_in_date DESC", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.NextResultAsync())
            {
                bookings.Add(Map(reader));
            }

            return bookings;
        }

        public async Task<List<Booking>> GetByRoomIdAsync(Guid roomId)
        {
            var bookings = new List<Booking>();
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM bookings WHERE room_id = @RoomId ORDER BY check_in_date DESC", conn);
            cmd.Parameters.AddWithValue("@RoomId", roomId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.NextResultAsync())
            {
                bookings.Add(Map(reader));
            }

            return bookings;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                SELECT COUNT(*) FROM bookings
                WHERE room_id = @RoomId
                AND status IN ('PENDING', 'CONFIRMED')
                AND (check_in_date, check_out_date) OVERLAPS (@CheckIn::timestamp, @CheckOut::timestamp)
            ", conn);

            cmd.Parameters.AddWithValue("@RoomId", roomId);
            cmd.Parameters.AddWithValue("@CheckIn", checkIn);
            cmd.Parameters.AddWithValue("@CheckOut", checkOut);

            var result = await cmd.ExecuteScalarAsync();
            return (long)result! == 0;
        }

        public async Task<long> CreateAsync(Booking booking)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO bookings
                (user_id, room_id, check_in_date, check_out_date, number_of_guests, total_price, status, notes)
                VALUES
                (@UserId, @RoomId, @CheckInDate, @CheckOutDate, @NumberOfGuests, @TotalPrice, @Status, @Notes)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", booking.UserId);
            cmd.Parameters.AddWithValue("@RoomId", booking.RoomId);
            cmd.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
            cmd.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
            cmd.Parameters.AddWithValue("@NumberOfGuests", booking.NumberOfGuests);
            cmd.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
            cmd.Parameters.AddWithValue("@Status", booking.Status);
            cmd.Parameters.AddWithValue("@Notes", (object?)booking.Notes ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return (long)result!;
        }

        public async Task UpdateStatusAsync(long bookingId, string status)
        {
            await using var conn = (NpgsqlConnection)_factory.CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("UPDATE bookings SET status = @Status, sys_updated = NOW() WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", bookingId);
            cmd.Parameters.AddWithValue("@Status", status);

            await cmd.ExecuteNonQueryAsync();
        }

        private Booking Map(IDataRecord r)
        {
            return new Booking
            {
                Id = (long)r["id"],
                UserId = (long)r["user_id"],
                RoomId = (Guid)r["room_id"],
                CheckInDate = (DateTime)r["check_in_date"],
                CheckOutDate = (DateTime)r["check_out_date"],
                NumberOfGuests = (int)r["number_of_guests"],
                TotalPrice = (decimal)r["total_price"],
                Status = r["status"].ToString()!,
                Notes = r["notes"] as string,
                SysCreated = (DateTime)r["sys_created"],
                SysUpdated = (DateTime)r["sys_updated"]
            };
        }
    }
}
