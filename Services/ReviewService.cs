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
        private readonly ReportRepository _reportRepository;

        public ReviewService(ReviewRepository reviewRepository, ReportRepository reportRepository)
        {
            _reviewRepository = reviewRepository;
            _reportRepository = reportRepository;
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


        public async Task<bool> DeleteReviewAsync(string id)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null) return false;

            // Supprimer la review
            await _reviewRepository.DeleteAsync(id);

            // Supprimer tous les signalements liés à cette review
            await _reportRepository.DeleteByReviewIdAsync(id);

            return true;
        }
    }
}
