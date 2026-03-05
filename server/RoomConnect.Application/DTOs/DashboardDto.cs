namespace RoomConnect.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalRooms { get; set; }
        public int ActiveRooms { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
    }
}
