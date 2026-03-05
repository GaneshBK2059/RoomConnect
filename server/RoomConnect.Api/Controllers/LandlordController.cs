using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomConnect.Application.Services;
using System.Security.Claims;

namespace RoomConnect.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "LANDLORD,ADMIN")]
    public class LandlordController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        private readonly ImageService _imageService;
        private readonly BookingService _bookingService;
        private readonly IWebHostEnvironment _env;

        public LandlordController(DashboardService dashboardService, ImageService imageService, BookingService bookingService, IWebHostEnvironment env)
        {
            _dashboardService = dashboardService;
            _imageService = imageService;
            _bookingService = bookingService;
            _env = env;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim?.Value, out var userId)) return Unauthorized();

            var stats = await _dashboardService.GetLandlordDashboardAsync(userId);
            return Ok(new { success = true, data = stats });
        }

        [HttpPost("rooms/{roomId}/images")]
        public async Task<IActionResult> UploadImages(Guid roomId, [FromForm] IFormFileCollection images)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim?.Value, out var userId)) return Unauthorized();

            if (images == null || images.Count == 0) return BadRequest(new { success = false, message = "No files uploaded" });

            var fileStreams = images.Select(f => (f.FileName, f.OpenReadStream() as Stream)).ToList();

            try
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var urls = await _imageService.UploadRoomImagesAsync(roomId, userId, webRoot, fileStreams);
                return Ok(new { success = true, data = urls });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim?.Value, out var userId)) return Unauthorized();
            
            var bookings = await _bookingService.GetLandlordBookingsAsync(userId);
            return Ok(new { success = true, data = bookings });
        }

        [HttpPatch("bookings/{id}/approve")]
        public async Task<IActionResult> ApproveBooking(long id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim?.Value, out var userId)) return Unauthorized();
            
            try
            {
                await _bookingService.UpdateBookingStatusByLandlordAsync(id, userId, "APPROVED");
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }

        [HttpPatch("bookings/{id}/reject")]
        public async Task<IActionResult> RejectBooking(long id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim?.Value, out var userId)) return Unauthorized();

            try
            {
                await _bookingService.UpdateBookingStatusByLandlordAsync(id, userId, "REJECTED");
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }
    }
}
