using MiniProjet.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Repositories
{
    public class ReviewRepository
    {
        private readonly IMongoCollection<Review> _reviews;

        public ReviewRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MiniProjet");
            _reviews = database.GetCollection<Review>("Reviews");
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _reviews.Find(r => true).ToListAsync();
        }

        public async Task<Review> GetByIdAsync(string id)
        {
            return await _reviews.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Review>> GetByPlaceIdAsync(string placeId)
        {
            return await _reviews.Find(r => r.PlaceId == placeId).ToListAsync();
        }

        public async Task<List<Review>> GetByUserIdAsync(string userId)
        {
            return await _reviews.Find(r => r.UserId == userId).ToListAsync();
        }

        public async Task CreateAsync(Review review)
        {
            await _reviews.InsertOneAsync(review);
        }

        public async Task UpdateAsync(string id, Review review)
        {
            await _reviews.ReplaceOneAsync(r => r.Id == id, review);
        }

        public async Task DeleteAsync(string id)
        {
            await _reviews.DeleteOneAsync(r => r.Id == id);
        }
    }
}
