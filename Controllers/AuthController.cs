using Microsoft.AspNetCore.Mvc;
using MiniProjet.Helpers;
using MiniProjet.Models;
using MiniProjet.Repositories;
using BCrypt.Net;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using MiniProjet.DTOs;

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
        public async Task<IActionResult> Register([FromBody] CreateUserDTO userDto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(userDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }
            // Mapper manuellement ou utiliser AutoMapper plus tard 😉
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
            };


            await _userRepository.CreateAsync(user);
            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Connexion d'un utilisateur existant.
        /// </summary>
        /// <param name="user">Données de l'utilisateur</param>
        /// <returns>JWT Token et informations de l'utilisateur ou erreur</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Connexion utilisateur", Description = "Vérifie les identifiants et renvoie un token JWT ainsi que les détails de l'utilisateur si valide.")]
        [SwaggerResponse(200, "Connexion réussie, retour du token et des détails utilisateur", typeof(object))]
        [SwaggerResponse(401, "Identifiants invalides", typeof(string))]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            // Vérifier si l'utilisateur existe
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser == null)
            {
                return Unauthorized(new { message = "Identifiants invalides. L'utilisateur n'existe pas." });
            }

            Console.WriteLine($"existingUser : {existingUser.Username}");

            // Vérification du mot de passe
            if (!BCrypt.Net.BCrypt.Verify(user.Password, existingUser.PasswordHash))
            {
                return Unauthorized(new { message = "Identifiants invalides. Le mot de passe est incorrect." });
            }

            // Génération du token JWT
            var token = _jwtHelper.GenerateToken(existingUser);

            // Construction de la réponse avec les détails de l'utilisateur
            var response = new
            {
                token,
                user = new
                {
                    id = existingUser.Id,
                    username = existingUser.Username,
                    email = existingUser.Email,
                    isAdmin = existingUser.IsAdmin,
                    photo = existingUser.Photo,
                    createdAt = existingUser.CreatedAt
                }
            };

            return Ok(response);
        }

    }
}
