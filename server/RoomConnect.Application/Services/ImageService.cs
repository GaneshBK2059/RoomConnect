using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Application.Services
{
    public class ImageService
    {
        private readonly RoomImagesRepository _roomImagesRepo;
        private readonly RoomRepository _roomRepo;

        public ImageService(RoomImagesRepository roomImagesRepo, RoomRepository roomRepo)
        {
            _roomImagesRepo = roomImagesRepo;
            _roomRepo = roomRepo;
        }

        public async Task<List<string>> UploadRoomImagesAsync(Guid roomId, long landlordId, string webRootPath, List<(string FileName, Stream Content)> files)
        {
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room == null || room.LandlordId != landlordId)
            {
                throw new UnauthorizedAccessException("Not authorized to upload images for this room");
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "rooms");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Content.Length == 0) continue;

                if (file.Content.Length > MaxFileSize)
                {
                    throw new InvalidOperationException($"File {file.FileName} exceeds the max size of 5MB.");
                }

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    throw new InvalidOperationException($"File {file.FileName} has an invalid extension.");
                }

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.Content.CopyToAsync(fileStream);
                }

                var imageUrl = $"/uploads/rooms/{uniqueFileName}";
                
                var roomImage = new RoomImage
                {
                    RoomId = roomId,
                    ImageUrl = imageUrl
                };
                
                await _roomImagesRepo.CreateAsync(roomImage);
                uploadedUrls.Add(imageUrl);
            }

            return uploadedUrls;
        }

        public async Task<List<RoomImage>> GetRoomImagesAsync(Guid roomId)
        {
            return await _roomImagesRepo.GetByRoomIdAsync(roomId);
        }
    }
}
