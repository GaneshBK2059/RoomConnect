using RoomConnect.Application.DTOs;
using RoomConnect.Application.Interfaces;
using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;

        public UserService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User?> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return null;
            }

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return null;
            }

            string hashedPassword = HashPassword(request.Password);

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                PasswordHash = hashedPassword,
                SysCreated = DateTime.UtcNow,
                SysUpdated = DateTime.UtcNow
            };

            long id = await _userRepo.CreateAsync(newUser);
            newUser.Id = id;

            return newUser;
        }

        public async Task<User?> ValidateUserAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return null;
            }

            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
                return null;

            bool isMatch = VerifyPassword(request.Password, user.PasswordHash);
            if (!isMatch)
                return null;

            return user;
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _userRepo.GetByIdAsync(userId);
        }

        private string HashPassword(string rawPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(rawPassword);
        }

        private bool VerifyPassword(string rawPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
        }
    }
}