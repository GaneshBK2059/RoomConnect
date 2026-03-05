using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomConnect.Application.DTOs;
using RoomConnect.Application.Services;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly RoomService _roomService;

    public RoomsController(RoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomService.GetAllAvailableRoomsAsync();
        return Ok(new { success = true, data = rooms });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)
            return NotFound(new { success = false, message = "Room not found" });

        return Ok(new { success = true, data = room });
    }

    [HttpGet("landlord/{landlordId}")]
    public async Task<IActionResult> GetLandlordRooms(long landlordId)
    {
        var rooms = await _roomService.GetLandlordRoomsAsync(landlordId);
        return Ok(new { success = true, data = rooms });
    }

    [HttpPost]
    [Authorize(Roles = "LANDLORD,ADMIN")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Invalid input" });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Unauthorized" });

        try
        {
            var roomId = await _roomService.CreateRoomAsync(userId, request);
            return Created($"/api/rooms/{roomId}", new { success = true, data = new { id = roomId } });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "LANDLORD,ADMIN")]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] CreateRoomRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Invalid input" });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Unauthorized" });

        try
        {
            await _roomService.UpdateRoomAsync(id, userId, request);
            return Ok(new { success = true, message = "Room updated successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "LANDLORD,ADMIN")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { success = false, message = "Unauthorized" });

        try
        {
            await _roomService.DeleteRoomAsync(id, userId);
            return Ok(new { success = true, message = "Room deleted successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
