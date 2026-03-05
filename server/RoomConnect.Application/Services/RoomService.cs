using RoomConnect.Application.DTOs;
using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Application.Services
{
    public class RoomService
    {
        private readonly RoomRepository _roomRepo;

        public RoomService(RoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public async Task<List<RoomDto>> GetAllAvailableRoomsAsync()
        {
            var rooms = await _roomRepo.GetAllAsync();
            return rooms.Select(MapToDto).ToList();
        }

        public async Task<RoomDto?> GetRoomByIdAsync(Guid id)
        {
            var room = await _roomRepo.GetByIdAsync(id);
            return room != null ? MapToDto(room) : null;
        }

        public async Task<List<RoomDto>> GetLandlordRoomsAsync(long landlordId)
        {
            var rooms = await _roomRepo.GetByLandlordIdAsync(landlordId);
            return rooms.Select(MapToDto).ToList();
        }

        public async Task<Guid> CreateRoomAsync(long landlordId, CreateRoomRequest request)
        {
            var room = new Room
            {
                LandlordId = landlordId,
                Title = request.Title,
                Description = request.Description,
                Address = request.Address,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Price = request.Price,
                RoomType = request.RoomType,
                MaxGuests = request.MaxGuests,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                WiFi = request.WiFi,
                Parking = request.Parking,
                AC = request.AC,
                Heating = request.Heating,
                ImageUrl = request.ImageUrl,
                IsAvailable = true
            };

            return await _roomRepo.CreateAsync(room);
        }

        public async Task UpdateRoomAsync(Guid roomId, long landlordId, CreateRoomRequest request)
        {
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room == null || room.LandlordId != landlordId)
            {
                throw new UnauthorizedAccessException("Not authorized to update this room");
            }

            room.Title = request.Title;
            room.Description = request.Description;
            room.Address = request.Address;
            room.City = request.City;
            room.State = request.State;
            room.ZipCode = request.ZipCode;
            room.Latitude = request.Latitude;
            room.Longitude = request.Longitude;
            room.Price = request.Price;
            room.RoomType = request.RoomType;
            room.MaxGuests = request.MaxGuests;
            room.Bedrooms = request.Bedrooms;
            room.Bathrooms = request.Bathrooms;
            room.WiFi = request.WiFi;
            room.Parking = request.Parking;
            room.AC = request.AC;
            room.Heating = request.Heating;
            room.ImageUrl = request.ImageUrl;

            await _roomRepo.UpdateAsync(room);
        }

        public async Task DeleteRoomAsync(Guid roomId, long landlordId)
        {
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room == null || room.LandlordId != landlordId)
            {
                throw new UnauthorizedAccessException("Not authorized to delete this room");
            }

            await _roomRepo.DeleteAsync(roomId);
        }

        private RoomDto MapToDto(Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                LandlordId = room.LandlordId,
                Title = room.Title,
                Description = room.Description,
                Address = room.Address,
                City = room.City,
                State = room.State,
                ZipCode = room.ZipCode,
                Latitude = room.Latitude,
                Longitude = room.Longitude,
                Price = room.Price,
                RoomType = room.RoomType,
                MaxGuests = room.MaxGuests,
                Bedrooms = room.Bedrooms,
                Bathrooms = room.Bathrooms,
                WiFi = room.WiFi,
                Parking = room.Parking,
                AC = room.AC,
                Heating = room.Heating,
                ImageUrl = room.ImageUrl,
                IsAvailable = room.IsAvailable,
                AvgRating = room.AvgRating,
                ReviewCount = room.ReviewCount,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }
    }
}
