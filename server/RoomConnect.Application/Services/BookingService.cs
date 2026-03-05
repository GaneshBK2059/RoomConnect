using RoomConnect.Application.DTOs;
using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Application.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepo;
        private readonly RoomRepository _roomRepo;

        public BookingService(BookingRepository bookingRepo, RoomRepository roomRepo)
        {
            _bookingRepo = bookingRepo;
            _roomRepo = roomRepo;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            return await _bookingRepo.IsRoomAvailableAsync(roomId, checkIn, checkOut);
        }

        public async Task<long> CreateBookingAsync(long userId, CreateBookingRequest request)
        {
            var room = await _roomRepo.GetByIdAsync(request.RoomId);
            if (room == null || !room.IsAvailable)
            {
                throw new InvalidOperationException("Room not available");
            }

            var isAvailable = await _bookingRepo.IsRoomAvailableAsync(request.RoomId, request.CheckInDate, request.CheckOutDate);
            if (!isAvailable)
            {
                throw new InvalidOperationException("Room not available for the selected dates");
            }

            if (request.NumberOfGuests > room.MaxGuests)
            {
                throw new InvalidOperationException($"Room can accommodate maximum {room.MaxGuests} guests");
            }

            var nights = (request.CheckOutDate - request.CheckInDate).Days;
            var totalPrice = nights * room.Price;

            var booking = new Booking
            {
                UserId = userId,
                RoomId = request.RoomId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                TotalPrice = totalPrice,
                Status = "PENDING",
                Notes = request.Notes
            };

            return await _bookingRepo.CreateAsync(booking);
        }

        public async Task<List<Booking>> GetUserBookingsAsync(long userId)
        {
            return await _bookingRepo.GetByUserIdAsync(userId);
        }

        public async Task CancelBookingAsync(long bookingId, long userId)
        {
            var bookings = await _bookingRepo.GetByUserIdAsync(userId);
            var booking = bookings.FirstOrDefault(b => b.Id == bookingId);
            
            if (booking == null)
            {
                throw new UnauthorizedAccessException("Not authorized");
            }

            await _bookingRepo.UpdateStatusAsync(bookingId, "CANCELLED");
        }

        public async Task<List<Booking>> GetLandlordBookingsAsync(long landlordId)
        {
            var rooms = await _roomRepo.GetByLandlordIdAsync(landlordId);
            var roomIds = rooms.Select(r => r.Id).ToList();

            var allBookings = new List<Booking>();
            foreach(var roomId in roomIds)
            {
                var roomBookings = await _bookingRepo.GetByRoomIdAsync(roomId);
                allBookings.AddRange(roomBookings);
            }

            return allBookings.OrderByDescending(b => b.CheckInDate).ToList();
        }

        public async Task UpdateBookingStatusByLandlordAsync(long bookingId, long landlordId, string status)
        {
            // First we need the booking. We can get it indirectly or load it.
            // Since BookingRepository currently doesn't have GetById, we can list all landlord bookings
            var bookings = await GetLandlordBookingsAsync(landlordId);
            var booking = bookings.FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                throw new UnauthorizedAccessException("Not authorized to update this booking");
            }

            await _bookingRepo.UpdateStatusAsync(bookingId, status);
        }
    }
}
