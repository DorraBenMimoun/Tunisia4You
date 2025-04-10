namespace MiniProjet.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public bool IsAdmin { get; set; }
    }
}
