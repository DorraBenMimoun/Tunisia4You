using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Repositories;
using MiniProjet.Services;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MiniProjet.Controllers
{
    [ApiController]
    [Route("/places")]
    [SwaggerTag("Gestion des lieux (restaurants, hôtels, sites touristiques, etc.)")]
    public class PlaceController : ControllerBase
    {
        private readonly PlaceService _placeService;
        private readonly TagRepository _tagRepository;
        private readonly ListeService _listeService;
        private readonly ReviewService _reviewService;
        private readonly PreferencesService _preferencesService;

        public PlaceController(
            PlaceService placeService,
            TagRepository tagRepository, 
            ListeService listeService,
            ReviewService reviewService,
            PreferencesService preferencesService)
        {
            _placeService = placeService;
            _tagRepository = tagRepository;
            _listeService = listeService;
            _reviewService = reviewService;
            _preferencesService = preferencesService;
        }

        /// <summary>
        /// Récupérer tous les lieux.
        /// </summary>
        /// <returns>Liste de tous les lieux enregistrés.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Récupérer tous les lieux", Description = "Retourne la liste de tous les lieux enregistrés.")]
        [SwaggerResponse(200, "Succès : Liste des lieux retournée.", typeof(List<Place>))]
        [SwaggerResponse(404, "Échec : Aucun lieu trouvé.")]
        public async Task<ActionResult<List<Place>>> GetAll()
        {
            var places = await _placeService.GetAllPlacesAsync();
            if (places == null || places.Count == 0)
            {
                return NotFound(new { message = "Aucun lieu trouvé." });
            }
            return Ok(new { message = "Liste des lieux récupérée avec succès.", data = places });
        }

        /// <summary>
        /// Récupérer un lieu spécifique par ID.
        /// </summary>
        /// <param name="id">L'ID du lieu à récupérer.</param>
        /// <returns>Le lieu correspondant.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Récupérer un lieu par ID", Description = "Retourne un lieu spécifique en fonction de son identifiant.")]
        [SwaggerResponse(200, "Succès : Lieu trouvé.", typeof(Place))]
        [SwaggerResponse(400, "Échec : ID invalide.")]
        [SwaggerResponse(404, "Échec : Lieu non trouvé.")]
        public async Task<ActionResult<Place>> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }

            var place = await _placeService.GetPlaceByIdAsync(id);

            if (place == null)
            {
                return NotFound(new { message = $"Aucun lieu trouvé avec l'ID {id}." });
            }

            return Ok(new { data = place });
        }

        /// <summary>
        /// Ajouter un nouveau lieu.
        /// </summary>
        /// <param name="dto">Les informations du lieu à créer.</param>
        /// <returns>Le lieu créé.</returns>
        [HttpPost("with-images")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Créer un lieu", Description = "Ajoute un nouveau lieu à la base de données. Accessible uniquement aux administrateurs.")]
        public async Task<ActionResult> Create([FromForm] CreatePlaceDTO dto)
        {

            var isAdmin = (bool?)HttpContext.Items["IsAdmin"];

            if (isAdmin != true)  // Vérifie si isAdmin est différent de true (c'est-à-dire, null ou false)
            {
                return Unauthorized(new { message = "Accès réservé aux administrateurs." });
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validatedTags = new List<string>();

            foreach (var tag in dto.Tags)
            {
                var existingTag = await _tagRepository.GetByLibelleAsync(tag);
                if (existingTag == null)
                {
                    var newTag = new TagPlace { Libelle = tag };
                    await _tagRepository.CreateAsync(newTag);
                    validatedTags.Add(tag);
                }
                else
                {
                    validatedTags.Add(existingTag.Libelle);
                }
            }

            dto.Tags = validatedTags;

            try
            {
                var createdPlace = await _placeService.CreatePlaceAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdPlace.Id }, new { message = "Lieu créé avec succès.", data = createdPlace });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur interne est survenue lors de la création du lieu.", details = ex.Message });
            }
        }



        /// <summary>
        /// Mettre à jour un lieu existant.
        /// </summary>
        /// <param name="id">L'ID du lieu à mettre à jour.</param>
        /// <param name="dto">Les nouvelles informations du lieu.</param>
        /// <returns>Un message de succès ou d'erreur.</returns>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Mettre à jour un lieu", Description = "Modifie les informations d'un lieu existant. Accessible uniquement aux administrateurs.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromForm] UpdatePlaceDTO dto)
        {
            var isAdmin = (bool?)HttpContext.Items["IsAdmin"];

            if (isAdmin != true)
            {
                return Unauthorized(new { message = "Accès réservé aux administrateurs." });
            }

            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "L'ID fourni n'est pas valide." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPlace = await _placeService.GetPlaceByIdAsync(id);
            if (existingPlace == null)
            {
                return NotFound(new { message = "Lieu non trouvé avec cet ID." });
            }

            var validatedTags = new List<string>();

            foreach (var tag in dto.Tags ?? new List<string>())
            {
                var existingTag = await _tagRepository.GetByLibelleAsync(tag);
                if (existingTag == null)
                {
                    var newTag = new TagPlace { Libelle = tag };
                    await _tagRepository.CreateAsync(newTag);
                    validatedTags.Add(tag);
                }
                else
                {
                    validatedTags.Add(existingTag.Libelle);
                }
            }

            dto.Tags = validatedTags;

            try
            {
                await _placeService.UpdatePlaceAsync(id, dto);
                return Ok(new { message = "Lieu mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur interne est survenue lors de la mise à jour du lieu.", details = ex.Message });
            }
        }

        /// <summary>
        /// Supprimer un lieu existant.
        /// </summary>
        /// <param name="id">L'ID du lieu à supprimer.</param>
        /// <returns>Un message de confirmation.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Supprimer un lieu", Description = "Supprime un lieu et le retire de toutes les listes associées.")]
        [SwaggerResponse(200, "Succès : Lieu supprimé avec succès.")]
        [SwaggerResponse(400, "Échec : ID invalide.")]
        [SwaggerResponse(404, "Échec : Lieu non trouvé.")]
        [SwaggerResponse(500, "Erreur interne du serveur.")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }

            var place = await _placeService.GetPlaceByIdAsync(id);
            if (place == null)
            {
                return NotFound(new { message = "Erreur : Le lieu avec cet ID n'existe pas." });
            }

            try
            {
                // 🔥 Supprimer le lieu de la base de données
                await _placeService.DeletePlaceAsync(id);

                // 🧹 Nettoyer les références dans les listes
                await _listeService.RemovePlaceFromAllListsAsync(id);

                return Ok(new { message = $"Succès : Le lieu avec l'ID {id} a été supprimé ainsi que toutes ses références dans les listes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression du lieu.", details = ex.Message });
            }
        }



        //get by category
        [HttpGet("category/{category}")]
        [SwaggerOperation(Summary = "Récupérer les lieux par catégorie", Description = "Retourne la liste des lieux en fonction de leur catégorie.")]
        [SwaggerResponse(200, "Succès : Liste des lieux retournée.", typeof(List<Place>))]
        [SwaggerResponse(404, "Échec : Aucun lieu trouvé.")]
        public async Task<ActionResult<List<Place>>> GetByCategory(string category)
        {
            var places = await _placeService.GetPlacesByCategoryAsync(category);
            if (places == null || places.Count == 0)
            {
                return NotFound(new { message = "Aucun lieu trouvé." });
            }
            return Ok(new { message = "Liste des lieux récupérée avec succès.", data = places });
        }

        //get by name
        [HttpGet("name/{name}")]
        [SwaggerOperation(Summary = "Récupérer les lieux par nom", Description = "Retourne la liste des lieux en fonction de leur nom.")]
        [SwaggerResponse(200, "Succès : Liste des lieux retournée.", typeof(List<Place>))]
        [SwaggerResponse(404, "Échec : Aucun lieu trouvé.")]
        public async Task<ActionResult<List<Place>>> GetByName(string name)
        {
            var places = await _placeService.GetPlacesByNameAsync(name);
            if (places == null || places.Count == 0)
            {
                return NotFound(new { message = "Aucun lieu trouvé." });
            }
            return Ok(new { message = "Liste des lieux récupérée avec succès.", data = places });
        }

        //get by tag
        [HttpGet("tag/{tag}")]
        [SwaggerOperation(Summary = "Récupérer les lieux par tags", Description = "Retourne la liste des lieux en fonction de leur tag.")]
        [SwaggerResponse(200, "Succès : Liste des lieux retournée.", typeof(List<Place>))]
        [SwaggerResponse(404, "Échec : Aucun lieu trouvé.")]
        public async Task<ActionResult<List<Place>>> GetByTag(string tag)
        {
            var places = await _placeService.GetPlacesByTagAsync(tag);
            if (places == null || places.Count == 0)
            {
                return NotFound(new { message = "Aucun lieu trouvé." });
            }
            return Ok(new { message = "Liste des lieux récupérée avec succès.", data = places });
        }

        /// <summary>
        /// Recherche avancée de lieux avec plusieurs critères.
        /// </summary>
        /// <param name="name">Nom du lieu (optionnel)</param>
        /// <param name="tags">Tags à rechercher (optionnel)</param>
        /// <param name="category">Catégorie du lieu (optionnel)</param>
        /// <param name="minRating">Note minimale (optionnel)</param>
        /// <param name="city">Ville du lieu (optionnel)</param>
        /// <returns>Liste des lieux correspondant aux critères de recherche</returns>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Recherche avancée de lieux", Description = "Recherche des lieux en fonction de plusieurs critères")]
        [SwaggerResponse(200, "Succès : Liste des lieux retournée.", typeof(List<Place>))]
        [SwaggerResponse(404, "Échec : Aucun lieu trouvé.")]
        public async Task<ActionResult<List<Place>>> SearchPlaces(
            [FromQuery] string? name = null,
            [FromQuery] List<string>? tags = null,
            [FromQuery] string? category = null,
            [FromQuery] double? minRating = null,
            [FromQuery] string? city = null)
            
        {
            var places = await _placeService.GetAllPlacesAsync();
            
            if (places == null || places.Count == 0)
            {
                return NotFound(new { message = "Aucun lieu trouvé." });
            }

            // Filtrage des résultats
            var filteredPlaces = places.Where(p => 
                (string.IsNullOrEmpty(name) || p.Name.ToLower().Contains(name.ToLower())) &&
                (string.IsNullOrEmpty(category) || p.Category.ToLower() == category.ToLower()) &&
                (string.IsNullOrEmpty(city) || p.City?.ToLower() == city.ToLower()) &&
                (!minRating.HasValue || p.AverageRating >= minRating.Value) &&
                (tags == null || tags.Count == 0 || p.Tags.Any(t => tags.Any(tag => tag.ToLower() == t.ToLower())))
            ).ToList();

            if (filteredPlaces.Count == 0)
            {
                return NotFound(new { message = "Aucun lieu ne correspond aux critères de recherche." });
            }

            return Ok(new { 
                message = "Recherche effectuée avec succès.", 
                data = filteredPlaces,
                count = filteredPlaces.Count
            });
        }

        /// <summary>
        /// Obtenir des recommandations de lieux basées sur l'historique des avis de l'utilisateur.
        /// </summary>
        /// <returns>Liste des lieux recommandés</returns>
        [HttpGet("recommandations")]
        [SwaggerOperation(Summary = "Obtenir des recommandations personnalisées", Description = "Retourne une liste de lieux recommandés basée sur les préférences de l'utilisateur et son historique d'avis.")]
        public async Task<ActionResult<List<Place>>> GetRecommandations()
        {
            // Récupérer l'ID de l'utilisateur connecté
            var userId = HttpContext.Items["user_id"]?.ToString();
          
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Utilisateur non authentifié." });
            }

            // Récupérer les préférences de l'utilisateur
            var userPreferences = await _preferencesService.GetUserPreferences(userId);
            if (userPreferences == null)
            {
                return BadRequest(new { message = "Veuillez d'abord configurer vos préférences pour recevoir des recommandations personnalisées." });
            }

            // Récupérer tous les lieux
            var allPlaces = await _placeService.GetAllPlacesAsync();
            
            // Filtrer les lieux selon les préférences de l'utilisateur
            var filteredPlaces = allPlaces.Where(p => 
                // Vérifier les villes préférées
                userPreferences.PreferredCities.Contains(p.City) &&
                // Vérifier les catégories préférées
                userPreferences.PreferredCategories.Contains(p.Category) &&
                // Vérifier la note minimale
                p.AverageRating >= userPreferences.MinRating &&
                // Vérifier les tags préférés (au moins un tag en commun)
                p.Tags.Any(t => userPreferences.PreferredTags.Contains(t))
            ).ToList();

            // Si aucun lieu ne correspond aux préférences, retourner un message approprié
            if (!filteredPlaces.Any())
            {
                return NotFound(new { message = "Aucun lieu ne correspond exactement à vos préférences. Essayez d'élargir vos critères." });
            }

            // Récupérer les avis récents et positifs de l'utilisateur
            var recentReviews = await _reviewService.GetRecentPositiveReviewsAsync(userId, 10);
            var ratedPlaceIds = recentReviews?.Select(r => r.PlaceId).ToHashSet() ?? new HashSet<string>();

            // Exclure les lieux déjà notés par l'utilisateur
            var recommendations = filteredPlaces
                .Where(p => !ratedPlaceIds.Contains(p.Id))
                .OrderByDescending(p => p.AverageRating)
                .Take(10)
                .ToList();

            if (!recommendations.Any())
            {
                return NotFound(new { message = "Vous avez déjà noté tous les lieux correspondant à vos préférences." });
            }

            return Ok(new { 
                message = "Recommandations générées avec succès.", 
                data = recommendations,
                count = recommendations.Count
            });
        }

    }
}
