using RoomConnect.Application.DTOs;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Application.Services
{
    public class DashboardService
    {
        private readonly RoomRepository _roomRepo;
        private readonly BookingRepository _bookingRepo;

        public DashboardService(RoomRepository roomRepo, BookingRepository bookingRepo)
        {
            _roomRepo = roomRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<DashboardDto> GetLandlordDashboardAsync(long landlordId)
        {
            var rooms = await _roomRepo.GetByLandlordIdAsync(landlordId);
            
            int totalRooms = rooms.Count;
            int activeRooms = rooms.Count(r => r.IsAvailable);

            var bookings = new List<Domain.Entities.Booking>();
            foreach (var room in rooms)
            {
                var roomBookings = await _bookingRepo.GetByRoomIdAsync(room.Id);
                bookings.AddRange(roomBookings);
            }

            int totalBookings = bookings.Count;
            int pendingBookings = bookings.Count(b => b.Status == "PENDING");

            return new DashboardDto
            {
                TotalRooms = totalRooms,
                ActiveRooms = activeRooms,
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings
            };
        }
    }
}
