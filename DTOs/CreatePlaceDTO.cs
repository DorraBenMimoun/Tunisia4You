namespace MiniProjet.DTOs
{
    public class CreatePlaceDTO
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? MapUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public Dictionary<string, string> OpeningHours { get; set; }
        public List<string> Tags { get; set; }
        
        public List<IFormFile>? Images { get; set; } 
    }
}
