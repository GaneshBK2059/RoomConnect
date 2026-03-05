namespace RoomConnect.Domain.Entities
{
    public class Review
    {
        public long Id { get; set; }
        public Guid RoomId { get; set; }
        public long UserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Title { get; set; } = "";
        public string Comment { get; set; } = "";
        public DateTime SysCreated { get; set; }
        public DateTime SysUpdated { get; set; }
    }
}
