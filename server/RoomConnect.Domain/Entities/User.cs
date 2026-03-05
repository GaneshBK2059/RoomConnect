namespace RoomConnect.Domain.Entities
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; } = "";

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the phone number of the user, if available.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the user's password.
        /// </summary>
        public string PasswordHash { get; set; } = "";

        /// <summary>
        /// Gets or sets the role of the user within the system.
        /// </summary>
        public string Role { get; set; } = "RENTER";

        /// <summary>
        /// Gets or sets a value indicating whether the user is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user's email address has been verified.
        /// </summary>
        public bool EmailVerified { get; set; } = false;

        /// <summary>
        /// Gets or sets the URL of the user's avatar image, if available.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets a brief bio or description of the user, if available.
        /// </summary>
        public string? Bio { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user record was created.
        /// </summary>
        public DateTime SysCreated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user record was last updated.
        /// </summary>
        public DateTime SysUpdated { get; set; }
    }
}
