namespace MiniProjet.DTOs
{
    public class UserStatsDTO
    {
        public int TotalUsers { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int BannedUsers { get; set; }
    }
}
