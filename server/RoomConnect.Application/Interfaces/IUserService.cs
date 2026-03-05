using RoomConnect.Application.DTOs;
using RoomConnect.Domain.Entities;

namespace RoomConnect.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(RegisterRequest request);
        Task<User?> ValidateUserAsync(LoginRequest request);
    }
}