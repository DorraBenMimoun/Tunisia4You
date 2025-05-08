using MiniProjet.Models;

namespace MiniProjet.DTOs
{
    public class PlaceStatsDTO
    {
        public int TotalPlaces { get; set; }
        public Dictionary<string, int> PlacesByCategory { get; set; }
        public List<Place> TopRatedPlaces { get; set; }
        public List<Place> WorstRatedPlaces { get; set; }
    }
}
