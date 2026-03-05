namespace RoomConnect.Application.DTOs
{
    public class CreateRoomRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Price { get; set; }
        public string RoomType { get; set; } = "Entire Place";
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public bool WiFi { get; set; }
        public bool Parking { get; set; }
        public bool AC { get; set; }
        public bool Heating { get; set; }
        public string ImageUrl { get; set; } = "";
    }
}
