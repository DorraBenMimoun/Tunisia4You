using MiniProjet.Models;
using MongoDB.Driver;


namespace MiniProjet.Repositories
{
    public class PlaceRepository
    {
        private readonly IMongoCollection<Place> _placesCollection;

        public PlaceRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MiniProjet");

            _placesCollection = database.GetCollection<Place>("places");
        }

        public async Task<List<Place>> GetAllAsync()
        {
            return await _placesCollection.Find(place => true).ToListAsync();
        }

        public async Task<Place> GetByIdAsync(string id)
        {
            return await _placesCollection.Find(place => place.Id == id).FirstOrDefaultAsync();
        }


        public async Task<Place> CreateAsync(Place place)
        {
            await _placesCollection.InsertOneAsync(place);
            return place;
        }

        public async Task<bool> UpdateAsync(string id, Place place)
        {
            var result = await _placesCollection.ReplaceOneAsync(p => p.Id == id, place);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _placesCollection.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Place>> GetByCategoryAsync(string category)
        {
            return await _placesCollection.Find(place => place.Category == category).ToListAsync();
        }

        //get by tag
        public async Task<List<Place>> GetByTagAsync(string tag)
        {
            return await _placesCollection.Find(place => place.Tags.Contains(tag)).ToListAsync();
        }

        //get by name
        public async Task<List<Place>> GetByNameAsync(string name)
        {
            return await _placesCollection.Find(place => place.Name == name).ToListAsync();
        }

        public async Task RemoveTagFromAllPlacesAsync(string tag)
        {
            var update = Builders<Place>.Update.Pull(p => p.Tags, tag);
            await _placesCollection.UpdateManyAsync(p => p.Tags.Contains(tag), update);
        }


    }
}
