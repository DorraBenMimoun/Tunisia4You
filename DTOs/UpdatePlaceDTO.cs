namespace MiniProjet.DTOs
{
    public class UpdatePlaceDTO
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
        public Dictionary<string, string>? OpeningHours { get; set; }
        public List<string>? Tags { get; set; }

        public List<IFormFile>? Images { get; set; } 

    }
}
