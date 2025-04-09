using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models;
using MiniProjet.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Controllers
{
    [Route("/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Récupère tous les tags disponibles.
        /// </summary>
        /// <returns>Une liste de tags.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Récupérer tous les tags", Description = "Retourne la liste de tous les tags disponibles.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TagPlace>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TagPlace>>> GetAllTags()
        {
            var tags = await _tagService.GetAllTagsAsync();
            if (tags == null || tags.Count == 0)
            {
                return NotFound(new { message = "Aucun tag trouvé." });
            }
            return Ok(new { message = "Liste des tags récupérée avec succès.", data = tags });
        }

        /// <summary>
        /// Récupère un tag spécifique en fonction de son ID.
        /// </summary>
        /// <param name="id">L'ID du tag à récupérer.</param>
        /// <returns>Le tag correspondant à l'ID.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Récupérer un tag par son ID", Description = "Renvoie un tag spécifique si l'ID est valide.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagPlace))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagPlace>> GetTagById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound(new { message = $"Tag avec l'ID {id} introuvable." });
            }
            return Ok(new { message = "Tag trouvé avec succès.", data = tag });
        }

        /// <summary>
        /// Crée un nouveau tag.
        /// </summary>
        /// <param name="TagPlace">Les données du tag à créer.</param>
        /// <returns>Le tag créé.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Créer un nouveau tag", Description = "Ajoute un nouveau tag avec un libellé unique à la base de données.")]
        [SwaggerResponse(200, "Tag ajouté avec succès", typeof(TagPlace))]
        [SwaggerResponse(400, "Le libellé du tag doit être unique", typeof(string))]
        public async Task<ActionResult> CreateTag([FromBody] TagPlace tag)
        {
            if (tag == null || string.IsNullOrWhiteSpace(tag.Libelle))
            {
                return BadRequest(new { message = "Le libellé du tag ne peut pas être vide." });
            }
            try
            {
                await _tagService.CreateTagAsync(tag);
                return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, new { message = "Tag créé avec succès.", data = tag });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Met à jour un tag existant.
        /// </summary>
        /// <param name="id">L'ID du tag à mettre à jour.</param>
        /// <param name="TagPlace">Les nouvelles données du tag.</param>
        /// <returns>Le tag mis à jour.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Mettre à jour un tag", Description = "Modifie les informations d'un tag existant.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagPlace))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateTag(string id, [FromBody] TagPlace tag)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }

            var existingTag = await _tagService.GetTagByIdAsync(id);
            if (existingTag == null)
            {
                return NotFound(new { message = $"Tag avec l'ID {id} introuvable." });
            }

            if (string.IsNullOrWhiteSpace(tag.Libelle))
            {
                return BadRequest(new { message = "Le libellé du tag ne peut pas être vide." });
            }

            await _tagService.UpdateTagAsync(id, tag);
            return Ok(new { message = "Tag mis à jour avec succès.", data = tag });
        }

        /// <summary>
        /// Supprime un tag en fonction de son ID.
        /// </summary>
        /// <param name="id">L'ID du tag à supprimer.</param>
        /// <returns>Un message de confirmation.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Supprimer un tag", Description = "Supprime un tag de la base de données.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTag(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Erreur : L'ID fourni n'est pas valide." });
            }

            var existingTag = await _tagService.GetTagByIdAsync(id);
            if (existingTag == null)
            {
                return NotFound(new { message = $"Tag avec l'ID {id} introuvable." });
            }

            await _tagService.DeleteTagAsync(id);
            return Ok(new { message = "Tag supprimé avec succès." });
        }

        /// <summary>
        /// Récupère un tag spécifique en fonction de son ID.
        /// </summary>
        /// <param name="libelle">Le libellé du tag à récupérer.</param>
        /// <returns>Le tag correspondant à le libelle.</returns>
        [HttpGet("search/{libelle}")]
        [SwaggerOperation(Summary = "Récupérer un tag par son libelle", Description = "Renvoie un tag spécifique si libelle est valide.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagPlace))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<TagPlace>> GetTagByLibelleAsync(string libelle)
        {
            if (string.IsNullOrWhiteSpace(libelle))
            {
                return BadRequest(new { message = "Le libellé ne peut pas être vide." });
            }

            var tag = await _tagService.GetTagByLibelleAsync(libelle);
            if (tag == null)
            {
                return NotFound(new { message = $"Tag avec le libellé '{libelle}' introuvable." });
            }

            return Ok(new { message = "Tag trouvé avec succès.", data = tag });
        }

    }

    
}
