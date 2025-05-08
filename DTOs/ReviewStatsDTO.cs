namespace MiniProjet.DTOs
{
    public class ReviewStatsDTO
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int RecentReviewsCount { get; set; }
        public int ReportedReviewsCount { get; set; }
    }
}
