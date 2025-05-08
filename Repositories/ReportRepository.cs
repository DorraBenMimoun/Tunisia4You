using MiniProjet.Models;
using MongoDB.Driver;

namespace MiniProjet.Repositories
{
    public class ReportRepository
    {
        private readonly IMongoCollection<Report> _reports;

        public ReportRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("MiniProjet");
            _reports = database.GetCollection<Report>("Reports");
        }

        public async Task CreateAsync(Report report)
        {
            await _reports.InsertOneAsync(report);
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _reports.Find(_ => true).ToListAsync();
        }

        public async Task<Report> GetByIdAsync(string id)
        {
            return await _reports.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Report>> GetByReviewIdAsync(string reviewId)
        {
            return await _reports.Find(r => r.ReviewId == reviewId).ToListAsync();
        }

        public async Task<List<Report>> GetByUserIdAsync(string userId)
        {
            return await _reports.Find(r => r.UserId == userId).ToListAsync();
        }

        //get report by user wich is reported who create review
        public async Task<List<Report>> GetByReportedUserIdAsync(string reportedUserId)
        {
            return await _reports.Find(r => r.ReportedUserId == reportedUserId).ToListAsync();
        }

        public async Task DeleteByReviewIdAsync(string reviewId)
        {
            await _reports.DeleteManyAsync(r => r.ReviewId == reviewId);
        }

        public async Task<int> CountReportsAsync()
{
            var count =  await _reports.CountDocumentsAsync(FilterDefinition<Report>.Empty);
            return (int)count;

        }

    }
}
