using MiniProjet.Models;


namespace MiniProjet.Services
{
    public interface IPlaceService
    {
        Task<List<Place>> GetAllPlacesAsync();
        Task<Place> GetPlaceByIdAsync(string id);
        Task<Place> CreatePlaceAsync(Place place);
        Task<bool> UpdatePlaceAsync(string id, Place place);
        Task<bool> DeletePlaceAsync(string id);
    }
}
