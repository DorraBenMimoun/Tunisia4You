using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models;
using MiniProjet.Services;
using MongoDB.Bson;

namespace MiniProjet.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // 🔹 Récupérer tous les utilisateurs
        [HttpGet] // Correspond à une requête GET sur "api/users"
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync(); // Appelle le service pour récupérer les utilisateurs
            return Ok(users); // Retourne le code HTTP 200 avec la liste des utilisateurs
        }

        // 🔹 Récupérer un utilisateur par ID
        [HttpGet("{id}")] // Correspond à une requête GET sur "api/users/{id}"
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id); // Recherche l'utilisateur par son ID

            if (user == null)
            {
                return NotFound(new { message = "Utilisateur introuvable" }); // Retourne 404 si l'utilisateur n'existe pas
            }

            return Ok(user); // Retourne 200 avec l'utilisateur trouvé
        }


        // 🔹 Mettre à jour un utilisateur
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            Console.WriteLine("Mise à jour de l'utilisateur en cours...");

            if (user == null)
            {
                return BadRequest(new { message = "Les données de l'utilisateur sont invalides. Veuillez fournir les informations nécessaires." });
            }

            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID fourni est invalide. Assurez-vous d'utiliser un identifiant correct." });
            }

            // Assurer que l'ID dans l'objet user correspond à celui fourni
            user.Id = objectId.ToString();

            bool updated = await _userService.UpdateUserAsync(id, user);

            if (!updated)
            {
                return NotFound(new { message = "Utilisateur non trouvé ou aucune modification apportée." });
            }

            return Ok(new { message = "Mise à jour réussie. Les informations de l'utilisateur ont été modifiées avec succès." });
        }

        // 🔹 Supprimer un utilisateur
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            // Vérifier si l'ID est un ObjectId valide
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });

            // Vérifier si l'utilisateur existe
            var user = _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Erreur : L'utilisateur avec cet ID n'existe pas." });

            // Supprimer l'utilisateur
            _userService.DeleteUserAsync(objectId);

            return Ok(new { message = $"Succès : L'utilisateur avec l'ID {id} a été supprimé avec succès." });
        }

    }

}
