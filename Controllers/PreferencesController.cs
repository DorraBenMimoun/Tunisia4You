using Microsoft.AspNetCore.Mvc;
using MiniProjet.Models;
using MiniProjet.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly PreferencesService _preferencesService;

        public PreferencesController(PreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Récupère les préférences d'un utilisateur")]
        [SwaggerResponse(200, "Préférences trouvées", typeof(Preferences))]
        [SwaggerResponse(404, "Aucune préférence trouvée")]
        public async Task<ActionResult<Preferences>> GetUserPreferences(string userId)
        {
            var preferences = await _preferencesService.GetUserPreferences(userId);
            if (preferences == null)
            {
                return NotFound();
            }
            return Ok(preferences);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Crée ou met à jour les préférences d'un utilisateur")]
        [SwaggerResponse(200, "Préférences créées ou mises à jour avec succès", typeof(Preferences))]
        public async Task<ActionResult<Preferences>> CreateOrUpdatePreferences(Preferences preferences)
        {
            var result = await _preferencesService.CreateOrUpdatePreferences(preferences);
            return Ok(result);
        }
    }
} 