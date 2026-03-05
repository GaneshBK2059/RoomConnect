using RoomConnect.Application.DTOs;
using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Application.Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _reviewRepo;

        public ReviewService(ReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<long> CreateReviewAsync(long userId, CreateReviewRequest request)
        {
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new InvalidOperationException("Rating must be between 1 and 5");
            }

            var review = new Review
            {
                RoomId = request.RoomId,
                UserId = userId,
                Rating = request.Rating,
                Title = request.Title,
                Comment = request.Comment
            };

            return await _reviewRepo.CreateAsync(review);
        }

        public async Task<List<Review>> GetRoomReviewsAsync(Guid roomId)
        {
            return await _reviewRepo.GetByRoomIdAsync(roomId);
        }
    }
}
