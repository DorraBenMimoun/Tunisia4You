using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjet.DTOs;
using MiniProjet.Models;
using MiniProjet.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MiniProjet.Controllers
{
    [Route("/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Signaler un avis comme inapproprié.
        /// </summary>
        [HttpPost]
        //[Authorize]
        [SwaggerOperation(Summary = "Signaler un avis", Description = "Permet à un utilisateur de signaler un avis.")]
        public async Task<IActionResult> Create([FromBody] CreateReportDTO dto)
        {
            /* var user = HttpContext.Items["User"] as User;
             if (user == null) return Unauthorized();
            */
            await _reportService.CreateReportAsync(dto);
            return Ok(new { message = "Avis signalé avec succès !" });
        }

        /// <summary>
        /// Voir tous les signalements (admin uniquement).
        /// </summary>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister tous les signalements", Description = "Accessible uniquement aux admins.")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _reportService.GetAllReports();
            return Ok(reports);
        }

        /// <summary>
        /// Voir les signalements d'un utilisateur spécifique (admin uniquement).
        /// </summary>
        [HttpGet("user/{userId}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister les signalements d'un utilisateur", Description = "Accessible uniquement aux admins.")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var reports = await _reportService.GetReportsByUserId(userId);
            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "Aucun signalement trouvé pour cet utilisateur." });
            }
            return Ok(reports);
        }

        /// <summary>
        /// Voir les signalements d'un avis spécifique (admin uniquement).
        /// </summary>
        [HttpGet("review/{reviewId}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister les signalements d'un avis", Description = "Accessible uniquement aux admins.")]
        public async Task<IActionResult> GetByReviewId(string reviewId)
        {
            var reports = await _reportService.GetReportsByReviewId(reviewId);
            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "Aucun signalement trouvé pour cet avis." });
            }
            return Ok(reports);
        }

        /// <summary>
        /// Voir les signalements d'un utilisateur spécifique (admin uniquement).
        /// </summary>
        [HttpGet("reported/{reportedUserId}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister les signalements d'un utilisateur", Description = "Accessible uniquement aux admins.")]
        public async Task<IActionResult> GetByReportedUserId(string reportedUserId)
        {
            var reports = await _reportService.GetReportsByReportedUserId(reportedUserId);
            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "Aucun signalement trouvé pour cet utilisateur." });
            }
            return Ok(reports);
        }

        /// <summary>
        /// Voir un signalement spécifique (admin uniquement).
        /// </summary>
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lister un signalement par ID", Description = "Accessible uniquement aux admins.")]
        public async Task<IActionResult> GetById(string id)
        {
            var report = await _reportService.GetReportById(id);
            if (report == null)
            {
                return NotFound(new { message = "Signalement introuvable." });
            }
            return Ok(report);
        }


        



    }
}
