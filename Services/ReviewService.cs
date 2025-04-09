using MiniProjet.Models;
using MiniProjet.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewService(ReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetAllAsync();
        }

        public async Task<Review> GetReviewByIdAsync(string id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

        public async Task<List<Review>> GetReviewsByPlaceIdAsync(string placeId)
        {
            return await _reviewRepository.GetByPlaceIdAsync(placeId);
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(string userId)
        {
            return await _reviewRepository.GetByUserIdAsync(userId);
        }

        public async Task CreateReviewAsync(Review review)
        {
            await _reviewRepository.CreateAsync(review);
        }

        public async Task UpdateReviewAsync(string id, Review review)
        {
            await _reviewRepository.UpdateAsync(id, review);
        }

        public async Task DeleteReviewAsync(string id)
        {
            await _reviewRepository.DeleteAsync(id);
        }
    }
}
