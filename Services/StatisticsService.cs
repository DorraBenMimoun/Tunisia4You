using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Repositories;
using MongoDB.Driver;

namespace MiniProjet.Services
{
    public class StatisticsService
    {
        private readonly IMongoCollection<User> _users;
        private readonly PlaceRepository _placeRepository;
        private readonly ReviewRepository _reviewRepository;
        private readonly ReportRepository _reportRepository;
        private readonly ListeRepository _listeRepository;

        public StatisticsService(IMongoClient mongoClient, PlaceRepository placeRepository, ReviewRepository reviewRepository, ReportRepository reportRepository, ListeRepository listeRepository)
        {
            var db = mongoClient.GetDatabase("MiniProjet");
            _users = db.GetCollection<User>("Users");
            _placeRepository = placeRepository;
            _reviewRepository = reviewRepository;
            _reportRepository = reportRepository;
            _listeRepository = listeRepository;
        }

        public async Task<UserStatsDTO> GetUserStatsAsync()
        {
            var now = DateTime.UtcNow;
            var oneWeekAgo = now.AddDays(-7);
            var oneMonthAgo = now.AddMonths(-1);

            var totalUsers = await _users.CountDocumentsAsync(_ => true);
            var newThisWeek = await _users.CountDocumentsAsync(u => u.CreatedAt >= oneWeekAgo);
            var newThisMonth = await _users.CountDocumentsAsync(u => u.CreatedAt >= oneMonthAgo);
            var bannedUsers = await _users.CountDocumentsAsync(u => u.DateFinBannissement != null && u.DateFinBannissement > now);

            return new UserStatsDTO
            {
                TotalUsers = (int)totalUsers,
                NewUsersThisWeek = (int)newThisWeek,
                NewUsersThisMonth = (int)newThisMonth,
                BannedUsers = (int)bannedUsers,
            };
        }

        public async Task<PlaceStatsDTO> GetPlaceStatisticsAsync()
        {
            var allPlaces = await _placeRepository.GetAllAsync();
            var now = DateTime.UtcNow;

            var stats = new PlaceStatsDTO
            {
                TotalPlaces = allPlaces.Count,

                PlacesByCategory = allPlaces
                    .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                    .GroupBy(p => NormalizeCategory(p.Category))
                    .ToDictionary(g => g.First().Category, g => g.Count()),

                TopRatedPlaces = allPlaces
                    .OrderByDescending(p => p.AverageRating)
                    .Take(5)
                    .ToList(),

                WorstRatedPlaces = allPlaces
                    .Where(p => p.ReviewCount > 0)
                    .OrderBy(p => p.AverageRating)
                    .Take(5)
                    .ToList()
            };

            return stats;
        }


        // 🔧 Normalise les catégories pour les comparer proprement
        private string NormalizeCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return string.Empty;

            // Supprime les accents
            string normalized = category.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            // Supprime les espaces superflus et met en minuscule
            return sb.ToString().ToLowerInvariant().Trim();
        }

        public async Task<ReviewStatsDTO> GetReviewStatsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            var now = DateTime.UtcNow;

            var recentDate = now.AddDays(-7);

            var stats = new ReviewStatsDTO
            {
                TotalReviews = reviews.Count,
                AverageRating = reviews.Count > 0 ? reviews.Average(r => r.Note) : 0,
                RecentReviewsCount = reviews.Count(r => r.CreatedAt.Date >= recentDate),
                ReportedReviewsCount = await _reportRepository.CountReportsAsync()
            };

            return stats;
        }

        public async Task<object> GetListeStatisticsAsync()
        {
            var allListes = await _listeRepository.GetAllAsync();
            var total = allListes.Count;

            if (total == 0)
            {
                return new
                {
                    TotalListes = 0,
                    PourcentagePubliques = "0%",
                    PourcentagePrivees = "0%",
                    MoyenneLieuxParListe = 0
                };
            }

            var publiques = allListes.Count(l => !l.IsPrivate);
            var privees = total - publiques;

            double moyenne = allListes.Average(l => l.LieuxIds?.Count ?? 0);

            return new
            {
                TotalListes = total,
                PourcentagePubliques = $"{(publiques * 100.0 / total):0.##}%",
                PourcentagePrivees = $"{(privees * 100.0 / total):0.##}%",
                MoyenneLieuxParListe = Math.Round(moyenne, 2)
            };
        }

    }
}
