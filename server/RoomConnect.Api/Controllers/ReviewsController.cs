using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomConnect.Application.DTOs;
using RoomConnect.Application.Services;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetRoomReviews(Guid roomId)
    {
        var reviews = await _reviewService.GetRoomReviewsAsync(roomId);
        return Ok(new { success = true, data = reviews });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Invalid input" });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Unauthorized" });

        try
        {
            var reviewId = await _reviewService.CreateReviewAsync(userId, request);
            return Created($"/api/reviews/{reviewId}", new { success = true, data = new { id = reviewId } });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
