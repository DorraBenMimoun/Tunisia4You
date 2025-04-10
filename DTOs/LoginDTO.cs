using System.ComponentModel.DataAnnotations;

namespace MiniProjet.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        public string Password { get; set; }
    }
}
