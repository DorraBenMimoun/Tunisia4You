using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models;
using MiniProjet.Services;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Controllers
{
    [Route("/users")]
    [ApiController]
    [SwaggerTag("Gestion des utilisateurs")] // Ajoute une description générale du contrôleur
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // 🔹 Récupérer tous les utilisateurs
        [HttpGet]
        [SwaggerOperation(Summary = "Récupérer tous les utilisateurs", Description = "Retourne la liste de tous les utilisateurs enregistrés.")]
        [SwaggerResponse(200, "Succès : Retourne la liste des utilisateurs.", typeof(IEnumerable<User>))]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // 🔹 Récupérer un utilisateur par ID
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Récupérer un utilisateur par ID", Description = "Retourne un utilisateur spécifique en fonction de son identifiant.")]
        [SwaggerResponse(200, "Succès : Retourne l'utilisateur demandé.", typeof(User))]
        [SwaggerResponse(400, "Échec : ID invalide.")]
        [SwaggerResponse(404, "Échec : Utilisateur non trouvé.")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Utilisateur introuvable" });
            }

            return Ok(user);
        }

        // 🔹 Mettre à jour un utilisateur
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Mettre à jour un utilisateur", Description = "Modifie les informations d'un utilisateur existant.")]
        [SwaggerResponse(200, "Succès : Mise à jour réussie.")]
        [SwaggerResponse(400, "Échec : ID invalide ou données incorrectes.")]
        [SwaggerResponse(404, "Échec : Utilisateur non trouvé.")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            Console.WriteLine("Mise à jour de l'utilisateur en cours...");

            if (user == null)
            {
                return BadRequest(new { message = "Les données de l'utilisateur sont invalides." });
            }

            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID fourni est invalide." });
            }

            user.Id = objectId.ToString();

            bool updated = await _userService.UpdateUserAsync(id, user);

            if (!updated)
            {
                return NotFound(new { message = "Utilisateur non trouvé ou aucune modification apportée." });
            }

            return Ok(new { message = "Mise à jour réussie." });
        }

        // 🔹 Supprimer un utilisateur
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Supprimer un utilisateur", Description = "Supprime un utilisateur en fonction de son identifiant.")]
        [SwaggerResponse(200, "Succès : L'utilisateur a été supprimé.")]
        [SwaggerResponse(400, "Échec : ID invalide.")]
        [SwaggerResponse(404, "Échec : Utilisateur non trouvé.")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Erreur : L'utilisateur avec cet ID n'existe pas." });

            await _userService.DeleteUserAsync(objectId);

            return Ok(new { message = $"Succès : L'utilisateur avec l'ID {id} a été supprimé." });
        }
    }
}
