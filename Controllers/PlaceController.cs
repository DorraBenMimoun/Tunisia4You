using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models;
using MiniProjet.Repositories;
using MiniProjet.Services;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Controllers
{
    [ApiController]
    [Route("/places")]
    [SwaggerTag("Gestion des lieux (restaurants, hôtels, sites touristiques, etc.)")]
    public class PlaceController : ControllerBase
    {
        private readonly PlaceService _placeService;
        private readonly TagRepository _tagPlaceRepository;

        public PlaceController(PlaceService placeService,TagRepository tagRepository)
        {
            _placeService = placeService;
            _tagPlaceRepository = tagRepository;
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

            return Ok(new { message = "Lieu trouvé avec succès.", data = place });
        }

        /// <summary>
        /// Ajouter un nouveau lieu.
        /// </summary>
        /// <param name="place">Les informations du lieu à créer.</param>
        /// <returns>Le lieu créé.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Créer un lieu", Description = "Ajoute un nouveau lieu à la base de données. Accessible uniquement aux administrateurs.")]
        [ProducesResponseType(typeof(object), 201)] // Succès
        [ProducesResponseType(typeof(object), 400)] // Mauvaise requête (données invalides)
        [ProducesResponseType(typeof(object), 401)] // Non authentifié
        [ProducesResponseType(typeof(object), 403)] // Non autorisé (non admin)
        public async Task<ActionResult<Place>> Create([FromBody] Place place)
        {
            if (place == null || place.Tags == null || place.Tags.Count == 0)
            {
                return BadRequest(new { message = "Les informations du lieu ou les tags sont invalides." });
            }

            var validatedTags = new List<string>(); // Liste des tags validés (déjà existants ou nouvellement créés)

            foreach (var tag in place.Tags)
            {
                var existingTag = await _tagPlaceRepository.GetByLibelleAsync(tag);
                if (existingTag == null)
                {
                    // Si le tag n'existe pas, on le crée et on l'ajoute à la base de données
                    var newTag = new TagPlace { Libelle = tag };
                    await _tagPlaceRepository.CreateAsync(newTag);
                    validatedTags.Add(tag);
                }
                else
                {
                    // Si le tag existe, on l'ajoute directement à la liste validée
                    validatedTags.Add(existingTag.Libelle);
                }
            }

            // Mise à jour des tags validés dans l'objet place
            place.Tags = validatedTags;

            try
            {
                var createdPlace = await _placeService.CreatePlaceAsync(place);
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
        /// <param name="place">Les nouvelles informations du lieu.</param>
        /// <returns>Un message de succès ou d'erreur.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Mettre à jour un lieu", Description = "Modifie les informations d'un lieu existant.")]
        [SwaggerResponse(200, "Succès : Lieu mis à jour avec succès.")]
        [SwaggerResponse(400, "Échec : ID invalide ou données incorrectes.")]
        [SwaggerResponse(404, "Échec : Lieu non trouvé.")]
        public async Task<IActionResult> Update(string id, [FromBody] Place place)
        {
            Console.WriteLine($"Tentative de mise à jour du lieu avec l'ID : {id}");

            if (place == null)
            {
                return BadRequest(new { message = "Les données du lieu sont invalides." });
            }

            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "L'ID fourni n'est pas valide." });
            }

            place.Id = objectId.ToString();

            bool updated = await _placeService.UpdateAsync(id, place);
            if (!updated)
            {
                return NotFound(new { message = "Le lieu n'a pas été trouvé ou aucune mise à jour nécessaire." });
            }

            return Ok(new { message = "Lieu mis à jour avec succès !" });
        }

        /// <summary>
        /// Supprimer un lieu existant.
        /// </summary>
        /// <param name="id">L'ID du lieu à supprimer.</param>
        /// <returns>Un message de confirmation.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Supprimer un lieu", Description = "Supprime un lieu en fonction de son identifiant.")]
        [SwaggerResponse(200, "Succès : Lieu supprimé avec succès.")]
        [SwaggerResponse(400, "Échec : ID invalide.")]
        [SwaggerResponse(404, "Échec : Lieu non trouvé.")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }

            var place = await _placeService.GetPlaceByIdAsync(id);
            Console.WriteLine($"place: {place}");

            if (place == null)
            {
                return NotFound(new { message = "Erreur : Le lieu avec cet ID n'existe pas." });
            }

            await _placeService.DeletePlaceAsync(id);
            return Ok(new { message = $"Succès : Le lieu avec l'ID {id} a été supprimé." });
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
        [SwaggerOperation(Summary = "Récupérer les lieux par tag", Description = "Retourne la liste des lieux en fonction de leur tag.")]
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



    }
}
