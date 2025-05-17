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
        private readonly UserRepository _userRepository; // Assurez-vous d'avoir une référence à UserRepository
        private readonly ReportRepository _reportRepository;
        private readonly PlaceRepository _placeRepository; // Assurez-vous d'avoir une référence à PlaceRepository

        public ReviewService(ReviewRepository reviewRepository, ReportRepository reportRepository, UserRepository userRepository, PlaceRepository placeRepository)

        {
            _reviewRepository = reviewRepository;
            _reportRepository = reportRepository;
            _userRepository = userRepository; // Initialisation de UserRepository
            _placeRepository = placeRepository;
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetAllAsync();
        }

        public async Task<Review> GetReviewByIdAsync(string id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

public async Task<List<ReviewWithUserDto>> GetReviewsWithUsersByPlaceIdAsync(string placeId)
{
    var reviews = await _reviewRepository.GetByPlaceIdAsync(placeId) ?? new List<Review>();

    var userIds = reviews.Select(r => r.UserId).Where(id => id != null).Distinct().ToList();
    var users = await _userRepository.GetManyByIdsAsync(userIds) ?? new List<User>();
        var usersDict = users
            .Where(u => !string.IsNullOrEmpty(u.Id))
            .ToDictionary(u => u.Id, u => u.Username);

    var result = reviews.Select(r => new ReviewWithUserDto
    {
        Id = r.Id,
        Commentaire = r.Commentaire,
        Note = r.Note,
        CreatedAt = r.CreatedAt,
        UserId = r.UserId,
        Username = (r.UserId != null && usersDict.ContainsKey(r.UserId)) ? usersDict[r.UserId] : "Inconnu"
    }).ToList();

    return result;
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

            // 1. Ajouter la nouvelle review
            await _reviewRepository.CreateAsync(review);

            // Recalculer les statistiques du lieu
            await RecalculatePlaceStatsAsync(dto.PlaceId);

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

            // Recalculer les statistiques du lieu
            await RecalculatePlaceStatsAsync(existingReview.PlaceId);

            return true;
        }



        public async Task<bool> DeleteReviewAsync(string id)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null) return false;

            // Supprimer la review
            await _reviewRepository.DeleteAsync(id);

            // Supprimer les reports associés
            await _reportRepository.DeleteByReviewIdAsync(id);

            // Recalculer les statistiques du lieu
            await RecalculatePlaceStatsAsync(existingReview.PlaceId);

            return true;
        }



        private async Task RecalculatePlaceStatsAsync(string placeId)
        {
            var reviews = await _reviewRepository.GetByPlaceIdAsync(placeId);
            double averageRating = reviews.Any() ? reviews.Average(r => r.Note) : 0;
            int reviewCount = reviews.Count;

            var place = await _placeRepository.GetByIdAsync(placeId);
            if (place != null)
            {
                place.AverageRating = averageRating;
                place.ReviewCount = reviewCount;
                await _placeRepository.UpdateAsync(place.Id, place);
            }
        }

        public async Task<List<Review>> GetRecentPositiveReviewsAsync(string userId, int limit)
        {
            return await _reviewRepository.GetRecentPositiveReviewsAsync(userId, limit);
        }

    }
}
