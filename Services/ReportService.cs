using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Repositories;

namespace MiniProjet.Services
{
    public class ReportService
    {
        private readonly ReportRepository _repository;

        public ReportService(ReportRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateReportAsync(CreateReportDTO dto)
        {
            var report = new Report
            {
                ReviewId = dto.ReviewId,
                ReportedUserId = dto.ReportedUserId,
                Reason = dto.Reason,
                UserId = dto.UserId
            };
            await _repository.CreateAsync(report);
        }

        public async Task<List<Report>> GetReportsByReviewId(string reviewId)
        {
            return await _repository.GetByReviewIdAsync(reviewId);
        }

        public async Task<List<Report>> GetAllReports()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Report>> GetReportsByUserId(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<List<Report>> GetReportsByReportedUserId(string reportedUserId)
        {
            return await _repository.GetByReportedUserIdAsync(reportedUserId);
        }

        public async Task<Report> GetReportById(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

     


    }
}
