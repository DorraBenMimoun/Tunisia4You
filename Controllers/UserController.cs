using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.DTOs;
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
        private readonly EmailService _emailService;
        private readonly ListeService _listeService;


        public UserController(UserService userService, EmailService emailService, ListeService listeService)
        {
            _userService = userService;
            _emailService = emailService;
            _listeService = listeService;
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

            var userTest = HttpContext.Items["User"] as User;
            if (userTest != null)
            {
                Console.WriteLine($"Utilisateur connecté : {userTest.Username}");
            }
            else
            {
                return Unauthorized(new { message = "Utilisateur non authentifié." });
            }

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
        [Authorize]
        [SwaggerOperation(Summary = "Mettre à jour un utilisateur", Description = "Modifie les informations d'un utilisateur existant.")]
        [SwaggerResponse(200, "Succès : Mise à jour réussie.")]
        [SwaggerResponse(400, "Échec : ID invalide ou données incorrectes.")]
        [SwaggerResponse(404, "Échec : Utilisateur non trouvé.")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO user)
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


            bool updated = await _userService.UpdateUserAsync(id, user);

            if (!updated)
            {
                return NotFound(new { message = "Utilisateur non trouvé ou aucune modification apportée." });
            }

            return Ok(new { message = "Mise à jour réussie." });
        }

        // 🔹 Supprimer un utilisateur
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Supprimer un utilisateur", Description = "Supprime un utilisateur et toutes ses listes associées.")]
        [SwaggerResponse(200, "Succès : L'utilisateur et ses listes ont été supprimés.")]
        [SwaggerResponse(400, "Échec : ID invalide.")]
        [SwaggerResponse(404, "Échec : Utilisateur non trouvé.")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Erreur : L'utilisateur avec cet ID n'existe pas." });

            try
            {
                // Supprimer les listes associées à l'utilisateur
                var listes = await _listeService.GetByCreateurIdAsync(id);
                if (listes.Any())
                {
                    foreach (var liste in listes)
                    {
                        await _listeService.DeleteAsync(liste.Id);
                    }
                    Console.WriteLine($"Listes associées à l'utilisateur {id} supprimées avec succès.");
                }

                // Supprimer l'utilisateur
                await _userService.DeleteUserAsync(objectId);

                return Ok(new { message = $"Succès : L'utilisateur avec l'ID {id} et ses listes associées ont été supprimés." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression de l'utilisateur et de ses listes.", details = ex.Message });
            }
        }


        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Demander une réinitialisation du mot de passe", Description = "Envoie un e-mail avec le lien de réinitialisation.")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new { message = "Aucun utilisateur trouvé avec cet e-mail." });

            var resetToken = Guid.NewGuid().ToString();
            await _userService.SetResetTokenAsync(user, resetToken);

            var resetLink = $"http://localhost:5066/reset-password?token={resetToken}";
            await _emailService.SendEmailAsync(user.Email, "Réinitialisation de mot de passe", $"Cliquez ici pour réinitialiser votre mot de passe : {resetLink}");

            return Ok(new { message = "E-mail de réinitialisation envoyé avec succès." });
        }


        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Réinitialiser le mot de passe", Description = "Réinitialise le mot de passe avec le token fourni.")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var user = await _userService.GetUserByResetTokenAsync(model.Token);
            if (user == null)
                return BadRequest(new { message = "Token invalide ou expiré." });

            await _userService.ResetPasswordAsync(user, model.NewPassword);

            return Ok(new { message = "Mot de passe réinitialisé avec succès." });
        }


        [HttpPatch("{id}/ban")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Bannir un utilisateur", Description = "Bannit un utilisateur jusqu’à une certaine date.")]
        public async Task<IActionResult> BannirUtilisateur(string id, [FromBody] DateTime dateFinBannissement)
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest(new { message = "ID utilisateur invalide." });

            var success = await _userService.BannirUtilisateurAsync(id, dateFinBannissement);
            if (!success)
                return NotFound(new { message = "Utilisateur introuvable." });

            return Ok(new { message = $"Utilisateur banni jusqu’au {dateFinBannissement:yyyy-MM-dd HH:mm:ss}." });
        }

        [HttpGet("bannis")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister les utilisateurs bannis", Description = "Retourne tous les utilisateurs encore bannis actuellement.")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUtilisateursBannis()
        {
            var users = await _userService.GetUtilisateursBannisAsync();
            return Ok(new { message = "Utilisateurs bannis récupérés avec succès", data = users });
        }

        /// <summary>
        /// Vérifie si un utilisateur est banni actuellement.
        /// </summary>
        [HttpGet("is-banni/{id}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Vérifier le statut de bannissement", Description = "Retourne true si l'utilisateur est actuellement banni.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> IsBanni(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Utilisateur introuvable." });

            var isBanni = user.DateFinBannissement.HasValue && user.DateFinBannissement > DateTime.UtcNow;
            return Ok(new { id = user.Id, isBanni });
        }

        /// <summary>
        /// Lister les utilisateurs non bannis.
        /// </summary>
        [HttpGet("non-bannis")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister les utilisateurs non bannis", Description = "Retourne les utilisateurs qui ne sont pas actuellement bannis.")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUtilisateursNonBannis()
        {
            var users = await _userService.GetUtilisateursNonBannisAsync();
            return Ok(new { message = "Utilisateurs non bannis récupérés avec succès.", data = users });
        }




    }
}
