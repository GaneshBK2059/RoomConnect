namespace RoomConnect.Domain.Entities
{
    public class RoomImage
    {
        public long Id { get; set; }
        public Guid RoomId { get; set; }
        public string ImageUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
