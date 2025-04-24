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

        public PlaceService(IMongoClient mongoClient, PlaceRepository placeRepository)
        {
            var database = mongoClient.GetDatabase("MiniProjet");
            _placesCollection = database.GetCollection<Place>("places");
            _placeRepository = placeRepository;
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
            var place = new Place
            {
                name = dto.Name,
                category = dto.Category,
                description = dto.Description,
                address = dto.Address,
                city = dto.City,
                latitude = dto.Latitude,
                longitude = dto.Longitude,
                phoneNumber = dto.PhoneNumber,
                openingHours = dto.OpeningHours,
                tags = dto.Tags ?? new List<string>(),
                images = dto.Images ?? new List<string>(),
                averageRating = 0,
                reviewCount = 0
            };

            await _placeRepository.CreateAsync(place);
            return place;
        }

        public async Task<bool> UpdatePlaceAsync(string id, UpdatePlaceDTO dto)
        {
            var place = await _placeRepository.GetByIdAsync(id);
            if (place == null) return false;

            place.name = dto.name ?? place.name;
            place.category = dto.category ?? place.category;
            place.description = dto.description ?? place.description;
            place.address = dto.address ?? place.address;
            place.city = dto.city ?? place.city;
            place.phoneNumber = dto.phoneNumber ?? place.phoneNumber;
            place.openingHours = dto.openingHours ?? place.openingHours;
            place.tags = dto.tags ?? place.tags;
            place.images = dto.images ?? place.images;

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
