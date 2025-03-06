using Microsoft.AspNetCore.Mvc;
using MiniProjet.Helpers;
using MiniProjet.Models;
using MiniProjet.Repositories;
using BCrypt.Net;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Controllers
{
    [Route("/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthController(UserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Inscription d'un nouvel utilisateur.
        /// </summary>
        /// <param name="user">Données de l'utilisateur</param>
        /// <returns>Message de succès ou erreur</returns>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Inscription d'un utilisateur", Description = "Crée un nouvel utilisateur avec un mot de passe haché.")]
        [SwaggerResponse(200, "Utilisateur enregistré avec succès", typeof(object))]
        [SwaggerResponse(400, "Nom d'utilisateur déjà existant", typeof(string))]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            // Hash du mot de passe
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _userRepository.CreateAsync(user);
            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Connexion d'un utilisateur existant.
        /// </summary>
        /// <param name="user">Données de l'utilisateur</param>
        /// <returns>JWT Token ou erreur</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Connexion utilisateur", Description = "Vérifie les identifiants et renvoie un token JWT si valide.")]
        [SwaggerResponse(200, "Connexion réussie, retour du token", typeof(object))]
        [SwaggerResponse(401, "Identifiants invalides", typeof(string))]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser == null)
            {
                return Unauthorized("Invalid credentials");
            }
            Console.WriteLine($"existingUser : {existingUser}");

            // Vérification du mot de passe
            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, existingUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtHelper.GenerateToken(existingUser);
            return Ok(new { token });
        }
    }
}
