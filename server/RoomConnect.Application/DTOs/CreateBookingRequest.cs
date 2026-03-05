namespace RoomConnect.Application.DTOs
{
    public class CreateBookingRequest
    {
        public Guid RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? Notes { get; set; }
    }
}
