using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Services;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Controllers
{
    [Route("/listes")]
    [ApiController]
    public class ListeController : ControllerBase
    {
        private readonly ListeService _listeService;
        private readonly PlaceService _placeService;
        private readonly UserService _userService;

        public ListeController(ListeService listeService, PlaceService placeService, UserService userService)
        {
            _listeService = listeService;
            _placeService = placeService;
            _userService = userService;
        }

        /// <summary>
        /// Récupérer toutes les listes.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Obtenir toutes les listes", Description = "Retourne la liste complète des listes.")]
        [ProducesResponseType(typeof(List<Liste>), 200)]
        public async Task<ActionResult<List<Liste>>> GetAll()
        {
            var listes = await _listeService.GetAllAsync();
            return Ok(listes);
        }

        /// <summary>
        /// Récupérer une liste par son ID.
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtenir une liste par ID", Description = "Retourne une liste spécifique en fonction de son ID.")]
        [ProducesResponseType(typeof(Liste), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Liste>> GetById(string id)
        {
            var liste = await _listeService.GetByIdAsync(id);
            if (liste == null)
                return NotFound("Liste non trouvée.");
            return Ok(liste);
        }

        /// <summary>
        /// Ajouter une nouvelle liste.
        /// </summary>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Créer une liste", Description = "Ajoute une nouvelle liste après validation.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateListeDTO createListeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Vérifier si une liste avec le même nom existe déjà
                var existingListe = await _listeService.GetByNameAsync(createListeDto.Nom);
                if (existingListe != null)
                {
                    return BadRequest(new { message = "Une liste avec ce nom existe déjà." });
                }

                var newListe = new CreateListeDTO
                {
                    Nom = createListeDto.Nom,
                    Description = createListeDto.Description,
                    IsPrivate = createListeDto.IsPrivate,
                    CreateurId = createListeDto.CreateurId,
                    LieuxIds = createListeDto.LieuxIds ?? new List<string>(),

                };

                await _listeService.CreateAsync(newListe);
                return CreatedAtAction(nameof(GetById), new { message = "Liste créée avec succès.", data = newListe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création de la liste.", details = ex.Message });
            }
        }




        /// <summary>
        /// Mettre à jour une liste existante.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Mettre à jour une liste", Description = "Modifie une liste existante en fonction de son ID.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateListeDTO updateListeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "L'ID fourni n'est pas valide." });
            }

            var existingListe = await _listeService.GetByIdAsync(id);
            if (existingListe == null)
            {
                return NotFound(new { message = $"Aucune liste trouvée avec l'ID {id}." });
            }

            try
            {
              

                await _listeService.UpdateAsync(id, updateListeDto);
                return Ok(new { message = "Liste mise à jour avec succès.", data = existingListe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Une erreur est survenue lors de la mise à jour de la liste.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Supprimer une liste par son ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Supprimer une liste", Description = "Supprime une liste par son ID.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(string id)
        {
            // Vérifier si l'ID est valide
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID fourni est invalide. Assurez-vous d'utiliser un identifiant MongoDB correct." });
            }

            // Vérifier si la liste existe
            var existingListe = await _listeService.GetByIdAsync(id);
            if (existingListe == null)
            {
                return NotFound(new { message = $"Aucune liste trouvée avec l'ID {id}." });
            }

            try
            {
                await _listeService.DeleteAsync(id);
                return Ok(new { message = $"La liste '{existingListe.Nom}' a été supprimée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression de la liste.", details = ex.Message });
            }
        }

        /// <summary>
        /// Récupérer les listes créées par un utilisateur.
        /// </summary>
        [HttpGet("createur/{createurId}")]
        [SwaggerOperation(Summary = "Obtenir les listes par créateur", Description = "Retourne toutes les listes créées par un utilisateur donné.")]
        [ProducesResponseType(typeof(List<Liste>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Liste>>> GetByCreateurId(string createurId)
        {
            Console.WriteLine($"[GetByCreateurId] Requête reçue pour le créateur : {createurId}");

            // Vérifier si l'ID est valide
            if (!ObjectId.TryParse(createurId, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID du créateur est invalide. Assurez-vous d'utiliser un identifiant MongoDB correct." });
            }

            try
            {
                // Vérifier si l'utilisateur existe
                var user = await _userService.GetUserByIdAsync(createurId);
                if (user == null)
                {
                    return NotFound(new { message = $"Aucun utilisateur trouvé avec l'ID {createurId}." });
                }

                // Récupérer les listes créées par l'utilisateur
                var listes = await _listeService.GetByCreateurIdAsync(createurId);

                if (listes == null || listes.Count == 0)
                {
                    return NotFound(new { message = $"Aucune liste trouvée pour l'utilisateur avec l'ID {createurId}." });
                }

                return Ok(new { message = $"Listes récupérées avec succès pour l'utilisateur {createurId}.", data = listes });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetByCreateurId] ❌ Erreur : {ex.Message}");
                return StatusCode(500, new { message = "Erreur lors de la récupération des listes.", details = ex.Message });
            }
        }

        /// <summary>
        /// Récupérer les listes publiques.
        /// </summary>
        [HttpGet("public")]
        [SwaggerOperation(Summary = "Obtenir les listes publiques", Description = "Retourne toutes les listes accessibles publiquement.")]
        [ProducesResponseType(typeof(List<Liste>), 200)]
        public async Task<ActionResult<List<Liste>>> GetPublic()
        {
            var listes = await _listeService.GetPublicAsync();
            return Ok(listes);
        }

        /// <summary>
        /// Rendre une liste publique.
        /// </summary>
        [HttpPatch("{id}/public")]
        [SwaggerOperation(Summary = "Rendre une liste publique", Description = "Modifie une liste pour la rendre publique.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> MakePublic(string id)
        {
            // Vérifier si l'ID est valide
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID fourni est invalide. Assurez-vous d'utiliser un identifiant MongoDB correct." });
            }

            try
            {
                // Vérifier si la liste existe
                var existingListe = await _listeService.GetByIdAsync(id);
                if (existingListe == null)
                {
                    return NotFound(new { message = $"Aucune liste trouvée avec l'ID {id}." });
                }

                // Vérifier si la liste est déjà publique
                if (!existingListe.IsPrivate)
                {
                    return BadRequest(new { message = "Cette liste est déjà publique." });
                }

                // Modifier la liste pour la rendre publique
                await _listeService.MakePublicAsync(id);
                return Ok(new { message = "La liste a été rendue publique avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur s'est produite lors de la mise à jour de la liste.", details = ex.Message });
            }
        }


        /// <summary>
        /// Rendre une liste privée.
        /// </summary>
        [HttpPatch("{id}/private")]
        [Authorize]
        [SwaggerOperation(Summary = "Rendre une liste privée", Description = "Modifie une liste pour la rendre privée.")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> MakePrivate(string id)
        {
            // Vérifier si l'ID est valide
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID fourni est invalide. Assurez-vous d'utiliser un identifiant MongoDB correct." });
            }

            try
            {
                // Vérifier si la liste existe
                var existingListe = await _listeService.GetByIdAsync(id);
                if (existingListe == null)
                {
                    return NotFound(new { message = $"Aucune liste trouvée avec l'ID {id}." });
                }

                // Vérifier si la liste est déjà private
                if (existingListe.IsPrivate)
                {
                    return BadRequest(new { message = "Cette liste est déjà privé." });
                }

                // Modifier la liste pour la rendre publique
                await _listeService.MakePrivateAsync(id);
                return Ok(new { message = "La liste a été rendue privée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur s'est produite lors de la mise à jour de la liste.", details = ex.Message });
            }
        }

        /// <summary>
        /// Ajouter un lieu à une liste.
        /// </summary>
        [HttpPost("{listeId}/places/{placeId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Ajouter un lieu à une liste", Description = "Ajoute un lieu spécifique à une liste.")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddPlace(string listeId, string placeId)
        {
            // Vérifier si l'ID du liste est valide
            if (!ObjectId.TryParse(listeId, out ObjectId objectIdListe))
            {
                return BadRequest(new { message = "L'ID du liste fourni est invalide. Assurez-vous d'utiliser un identifiant MongoDB correct." });
            }

            // Vérifier si l'ID est valide
            if (!ObjectId.TryParse(placeId, out ObjectId objectIdPlace))
            {
                return BadRequest(new { message = "L'ID du place fourni est invalide. Assurez-vous d'utiliser un identifiant MongoDB correct." });
            }

            try
            {
                // Vérifier si la liste existe
                var existingListe = await _listeService.GetByIdAsync(listeId);
                if (existingListe == null)
                {
                    return NotFound(new { message = $"Aucune liste trouvée avec l'ID {listeId}." });
                }

                // Vérifier si le lieux existe
                var existingPlace = await _placeService.GetPlaceByIdAsync(placeId);
                if (existingPlace == null)
                {
                    return NotFound(new { message = $"Aucune lieux trouvée avec l'ID {placeId}." });
                }



                await _listeService.AddPlaceAsync(listeId, placeId);
                return Ok(new { message = "Le lieux a été ajoutée avec succès à la liste." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur s'est produite lors de l'ajout du lieux à la liste.", details = ex.Message });
            }


        }

        /// <summary>
        /// Supprimer un lieu d'une liste.
        /// </summary>
        [HttpDelete("{listeId}/places/{placeId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Supprimer un lieu d'une liste", Description = "Supprime un lieu spécifique d'une liste.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemovePlace(string listeId, string placeId)
        {
            try
            {
                Console.WriteLine($"[RemovePlace] Requête pour supprimer le lieu {placeId} de la liste {listeId}");

                // Vérifier si la liste existe
                var existingListe = await _listeService.GetByIdAsync(listeId);
                if (existingListe == null)
                {
                    return NotFound(new { message = "Liste introuvable. Vérifiez l'ID de la liste." });
                }

                // Vérifier si le lieu existe dans la liste
                if (existingListe.LieuxIds == null || !existingListe.LieuxIds.Contains(placeId))
                {
                    return BadRequest(new { message = "Le lieu spécifié n'existe pas dans la liste." });
                }

                // Supprimer le lieu de la liste
                await _listeService.RemovePlaceAsync(listeId, placeId);

                return Ok(new { message = $"Le lieu avec l'ID {placeId} a été supprimé de la liste avec succès." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemovePlace] ❌ Erreur lors de la suppression du lieu : {ex.Message}");
                return StatusCode(500, new { message = "Erreur interne lors de la suppression du lieu.", details = ex.Message });
            }
        }


        /// <summary>
        /// Récupérer les lieux d'une liste.
        /// </summary>
        [HttpGet("{listeId}/places")]
        [SwaggerOperation(Summary = "Obtenir les lieux d'une liste", Description = "Retourne tous les lieux d'une liste spécifique.")]
        [ProducesResponseType(typeof(List<Place>), 200)]
        public async Task<ActionResult<List<Place>>> GetPlaces(string listeId)
        {
            var places = await _listeService.GetPlacesAsync(listeId);
            return Ok(places);
        }
    }
}
