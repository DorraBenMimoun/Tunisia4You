namespace MiniProjet.DTOs
{
    public class UpdatePlaceDTO
    {
        public string? name { get; set; }
        public string? category { get; set; }
        public string? description { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? phoneNumber { get; set; }
        public Dictionary<string, string>? openingHours { get; set; }
        public List<string>? tags { get; set; }
        public List<string>? images { get; set; }
    }
}
