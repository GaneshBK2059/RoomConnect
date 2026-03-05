using Microsoft.AspNetCore.Mvc;
using RoomConnect.Application.DTOs;
using RoomConnect.Application.Interfaces;
using RoomConnect.Application.Services;
using RoomConnect.Domain.Entities;
using RoomConnect.Infrastructure.Repositories;

namespace RoomConnect.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly RefreshTokenRepository _refreshTokenRepo;

        public AuthController(
            IUserService userService, 
            ITokenService tokenService, 
            RefreshTokenRepository refreshTokenRepo)
        {
            _userService = userService;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { Success = false, Message = "Invalid input" });

            var user = await _userService.RegisterAsync(request);
            if (user == null)
                return BadRequest(new AuthResponse { Success = false, Message = "Registration failed. Email may already exist." });

            var token = _tokenService.GenerateAccessToken(user);
            await SetupRefreshTokenForUser(user.Id);

            var userDto = MapToUserDto(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = userDto
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { Success = false, Message = "Invalid input" });

            var user = await _userService.ValidateUserAsync(request);
            if (user == null)
                return Unauthorized(new AuthResponse { Success = false, Message = "Invalid credentials" });

            var token = _tokenService.GenerateAccessToken(user);
            await SetupRefreshTokenForUser(user.Id);

            var userDto = MapToUserDto(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = userDto
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token is missing." });

            var tokenHash = _tokenService.HashToken(refreshToken);
            var refreshSession = await _refreshTokenRepo.GetByTokenStrAsync(tokenHash);

            if (refreshSession == null || !refreshSession.IsActive)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            // Revoke the old token (rotation)
            await _refreshTokenRepo.RevokeAsync(refreshSession.Id, DateTime.UtcNow);

            var user = await ((RoomConnect.Application.Services.UserService)_userService).GetUserByIdAsync(refreshSession.UserId);
            if (user == null || !user.IsActive)
                return Unauthorized(new { message = "User not found or inactive." });

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            await SetupRefreshTokenForUser(user.Id);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = newAccessToken,
                User = MapToUserDto(user)
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var tokenHash = _tokenService.HashToken(refreshToken);
                var refreshSession = await _refreshTokenRepo.GetByTokenStrAsync(tokenHash);

                if (refreshSession != null)
                {
                    await _refreshTokenRepo.RevokeAsync(refreshSession.Id, DateTime.UtcNow);
                }
            }

            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged out successfully" });
        }

        private async Task SetupRefreshTokenForUser(long userId)
        {
            var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
            var tokenHash = _tokenService.HashToken(newRefreshTokenStr);

            var refreshEntity = new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.CreateAsync(refreshEntity);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure HTTPS in production
                SameSite = SameSiteMode.Strict,
                Expires = refreshEntity.ExpiresAt
            };

            Response.Cookies.Append("refreshToken", newRefreshTokenStr, cookieOptions);
        }

        private UserDto MapToUserDto(RoomConnect.Domain.Entities.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio
            };
        }
    }
}