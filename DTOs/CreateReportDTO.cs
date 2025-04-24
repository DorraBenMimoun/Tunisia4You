namespace MiniProjet.DTOs
{
    public class CreateReportDTO
    {
        public string UserId { get; set; }

    
        public string ReportedUserId { get; set; }

        public string ReviewId { get; set; }
        public string Reason { get; set; }
    }
}
