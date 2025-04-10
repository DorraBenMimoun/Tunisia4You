using MiniProjet.DTOs;
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
        public async Task<Review> CreateReviewAsync(CreateReviewDTO dto)
        {
            var review = new Review
            {
                Commentaire = dto.Commentaire,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow,
                UserId = dto.UserId,
                PlaceId = dto.PlaceId
            };

            await _reviewRepository.CreateAsync(review);
            return review;
        }


        public async Task<bool> UpdateReviewAsync(string id, UpdateReviewDTO dto)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null) return false;

            existingReview.Commentaire = dto.Commentaire ?? existingReview.Commentaire;
            existingReview.Note = dto.Note != 0 ? dto.Note : existingReview.Note;
            existingReview.CreatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(id, existingReview);
            return true;
        }


        public async Task DeleteReviewAsync(string id)
        {
            await _reviewRepository.DeleteAsync(id);
        }
    }
}
