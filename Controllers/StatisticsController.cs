using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.DTOs;
using MiniProjet.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Controllers
{
    [Route("/stats")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Récupérer les statistiques globales des utilisateurs
        /// </summary>
        [HttpGet("users")]
        [SwaggerOperation(Summary = "Statistiques des utilisateurs", Description = "Récupère les données d'activité et d'inscription des utilisateurs.")]
        [ProducesResponseType(typeof(UserStatsDTO), 200)]
        public async Task<ActionResult<UserStatsDTO>> GetUserStats()
        {
            var stats = await _statisticsService.GetUserStatsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Récupérer les statistiques globales des lieux
        /// </summary>

        [HttpGet("places")]
        [SwaggerOperation(Summary = "Statistiques sur les lieux", Description = "Fournit des données agrégées sur les lieux.")]
        public async Task<IActionResult> GetPlaceStats()
        {
            var stats = await _statisticsService.GetPlaceStatisticsAsync();
            return Ok(stats);
        }


        /// <summary>
        /// Statistiques sur les avis (reviews).
        /// </summary>
        [HttpGet("reviews")]
        [SwaggerOperation(Summary = "Statistiques sur les avis", Description = "Fournit des statistiques sur les avis des utilisateurs.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReviewStats()
        {
            var stats = await _statisticsService.GetReviewStatsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Statistiques sur les listes
        /// </summary>

        [HttpGet("lists")]

        [SwaggerOperation(Summary = "Statistiques sur les listes", Description = "Fournit des statistiques sur les listes créées par les utilisateurs.")]  
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetListeStatistics()
        {
            var stats = await _statisticsService.GetListeStatisticsAsync();
            return Ok(stats);
        }

    }
}
