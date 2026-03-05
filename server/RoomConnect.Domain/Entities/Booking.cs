namespace RoomConnect.Domain.Entities
{
    public class Booking
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "PENDING"; // PENDING, CONFIRMED, CANCELLED, COMPLETED
        public string? Notes { get; set; }
        public DateTime SysCreated { get; set; }
        public DateTime SysUpdated { get; set; }
    }
}
