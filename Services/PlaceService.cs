using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;


namespace MiniProjet.Services
{
    public class PlaceService 
    {
        private readonly PlaceRepository _placeRepository;
        private readonly IMongoCollection<Place> _placesCollection; // Référence à la collection MongoDB
        private readonly ImageService _imageService; // Service pour gérer les images

        public PlaceService(IMongoClient mongoClient, PlaceRepository placeRepository, ImageService imageService)
        {
            var database = mongoClient.GetDatabase("MiniProjet");
            _placesCollection = database.GetCollection<Place>("places");
            _placeRepository = placeRepository;
            _imageService = imageService;

        }


        public async Task<List<Place>> GetAllPlacesAsync()
        {
            return await _placeRepository.GetAllAsync();
        }

        public async Task<Place> GetPlaceByIdAsync(string id)
        {
            return await _placeRepository.GetByIdAsync(id);
        }

        public async Task<Place> CreatePlaceAsync(CreatePlaceDTO dto)
        {

            var imageUrls = dto.Images != null ? await _imageService.SaveImagesAsync(dto.Images) : new List<string>();

            var place = new Place
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
            
                PhoneNumber = dto.PhoneNumber,
                OpeningHours = dto.OpeningHours,
                Tags = dto.Tags ?? new List<string>(),
                Images = imageUrls,
                AverageRating = 0,
                ReviewCount = 0
            };

            await _placeRepository.CreateAsync(place);
            return place;
        }

        public async Task<bool> UpdatePlaceAsync(string id, UpdatePlaceDTO dto)
    {
        var place = await _placeRepository.GetByIdAsync(id);
        if (place == null) return false;

        place.Name = dto.Name ?? place.Name;
        place.Category = dto.Category ?? place.Category;
        place.Description = dto.Description ?? place.Description;
        place.Address = dto.Address ?? place.Address;
        place.City = dto.City ?? place.City;
        place.PhoneNumber = dto.PhoneNumber ?? place.PhoneNumber;
        place.OpeningHours = dto.OpeningHours ?? place.OpeningHours;
        place.Tags = dto.Tags ?? place.Tags;
        place.MapUrl = dto.MapUrl ?? place.MapUrl;

        if (dto.Images != null && dto.Images.Count > 0)
        {
            var imageUrls = await _imageService.SaveImagesAsync(dto.Images);
            place.Images = imageUrls;
        }

        await _placeRepository.UpdateAsync(id, place);
        return true;
    }




        public async Task<bool> DeletePlaceAsync(string id)
        {
            return await _placeRepository.DeleteAsync(id);
        }


        public async Task<List<Place>> GetPlacesByCategoryAsync(string category)
        {
            return await _placeRepository.GetByCategoryAsync(category);
        }

        public async Task<List<Place>> GetPlacesByTagAsync(string tag)
        {
            return await _placeRepository.GetByTagAsync(tag);
        }

        public async Task<List<Place>> GetPlacesByNameAsync(string name)
        {
            return await _placeRepository.GetByNameAsync(name);
        }

    }
}
