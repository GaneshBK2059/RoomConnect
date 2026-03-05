namespace RoomConnect.Application.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string Role { get; set; } = "RENTER";
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
    }
}
