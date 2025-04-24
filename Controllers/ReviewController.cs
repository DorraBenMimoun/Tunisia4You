using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Services;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Controllers
{
    [Route("/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Récupérer toutes les reviews.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Récupérer toutes les reviews", Description = "Retourne la liste complète des avis.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(new { message = "Liste des avis récupérée avec succès.", data = reviews });
        }

        /// <summary>
        /// Récupérer une review par son ID.
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Récupérer une review par ID", Description = "Retourne un avis spécifique grâce à son ID.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "ID invalide. Assurez-vous que l'ID est au format MongoDB correct." });
            }

            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound(new { message = $"Aucun avis trouvé avec l'ID {id}." });
            }

            return Ok(new { message = "Avis trouvé avec succès.", data = review });
        }

        /// <summary>
        /// Récupérer les reviews d'un lieu spécifique.
        /// </summary>
        [HttpGet("place/{placeId}")]
        [SwaggerOperation(Summary = "Récupérer les reviews d'un lieu", Description = "Retourne la liste des avis pour un lieu donné.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByPlaceId(string placeId)
        {
            if (!ObjectId.TryParse(placeId, out _))
            {
                return BadRequest(new { message = "ID du lieu invalide." });
            }

            var reviews = await _reviewService.GetReviewsByPlaceIdAsync(placeId);
            return Ok(new { message = "Avis du lieu récupérés avec succès.", data = reviews });
        }

        /// <summary>
        /// Récupérer les reviews d'un utilisateur spécifique.
        /// </summary>
        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Récupérer les reviews d'un utilisateur", Description = "Retourne la liste des avis écrits par un utilisateur donné.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return BadRequest(new { message = "ID de l'utilisateur invalide." });
            }

            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(new { message = "Avis de l'utilisateur récupérés avec succès.", data = reviews });
        }

        /// <summary>
        /// Créer une nouvelle review.
        /// </summary>
        /// <summary>
        /// Créer une nouvelle review.
        /// </summary>
        [HttpPost]
        //[Authorize]
        [SwaggerOperation(Summary = "Créer une review", Description = "Ajoute un nouvel avis à la base de données.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateReviewDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdReview.Id }, new { message = "Avis créé avec succès.", data = createdReview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la création de l'avis.", details = ex.Message });
            }
        }

        /// <summary>
        /// Mettre à jour une review existante.
        /// </summary>
        /// <summary>
        /// Mettre à jour une review existante.
        /// </summary>
        [HttpPut("{id}")]
        //[Authorize]
        [SwaggerOperation(Summary = "Mettre à jour une review", Description = "Met à jour les informations d'une review existante.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReviewDTO dto)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "L'ID fourni n'est pas valide." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingReview = await _reviewService.GetReviewByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound(new { message = $"Review avec l'ID {id} introuvable." });
            }

            try
            {
                await _reviewService.UpdateReviewAsync(id, dto);
                return Ok(new { message = "Avis mis à jour avec succès.", data = existingReview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la mise à jour de l'avis.", details = ex.Message });
            }
        }

        /// <summary>
        /// Supprimer une review.
        /// </summary>
        [HttpDelete("{id}")]
        //[Authorize]
        [SwaggerOperation(Summary = "Supprimer une review", Description = "Supprime une review existante.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "ID de la review invalide." });
            }

            var deleted = await _reviewService.DeleteReviewAsync(id);
            if (!deleted)
                return NotFound(new { message = "Review introuvable." });

            return Ok(new { message = "Review et signalements supprimés avec succès." });
        }
    }
}
