namespace MiniProjet.DTOs
{
    public class UpdateUserDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Photo { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
