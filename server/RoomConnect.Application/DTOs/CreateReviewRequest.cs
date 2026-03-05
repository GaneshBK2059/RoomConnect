namespace RoomConnect.Application.DTOs
{
    public class CreateReviewRequest
    {
        public Guid RoomId { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; } = "";
        public string Comment { get; set; } = "";
    }
}
