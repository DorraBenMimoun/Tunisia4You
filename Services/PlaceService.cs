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

        public async Task<Place> CreatePlaceAsync(Place place)
        {
            return await _placeRepository.CreateAsync(place);
        }

       public async Task<bool> UpdateAsync(string id, Place place)
{
    if (!ObjectId.TryParse(id, out ObjectId objectId))
    {
        throw new FormatException("L'ID fourni n'est pas un ObjectId valide.");
    }

    // Récupérer l'ancien document depuis MongoDB
    var existingPlace = await _placesCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
    if (existingPlace == null)
    {
        return false; // Lieu introuvable
    }

    // Appliquer les nouvelles valeurs seulement si elles sont fournies
    existingPlace.Name = !string.IsNullOrWhiteSpace(place.Name) ? place.Name : existingPlace.Name;
    existingPlace.Category = !string.IsNullOrWhiteSpace(place.Category) ? place.Category : existingPlace.Category;
    existingPlace.Description = !string.IsNullOrWhiteSpace(place.Description) ? place.Description : existingPlace.Description;
    existingPlace.Address = !string.IsNullOrWhiteSpace(place.Address) ? place.Address : existingPlace.Address;
    existingPlace.PhoneNumber = !string.IsNullOrWhiteSpace(place.PhoneNumber) ? place.PhoneNumber : existingPlace.PhoneNumber;
    existingPlace.Latitude = place.Latitude != 0 ? place.Latitude : existingPlace.Latitude;
    existingPlace.Longitude = place.Longitude != 0 ? place.Longitude : existingPlace.Longitude;

    if (place.OpeningHours != null && place.OpeningHours.Count > 0)
        existingPlace.OpeningHours = place.OpeningHours;
    
    if (place.Tags != null && place.Tags.Count > 0)
        existingPlace.Tags = place.Tags;

    if (place.Images != null && place.Images.Count > 0)
        existingPlace.Images = place.Images;

    // Mise à jour dans la base de données
    var result = await _placesCollection.ReplaceOneAsync(p => p.Id == id, existingPlace);

    return result.ModifiedCount > 0;
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
