using FirstProjectAPI.Services.Dashboard;
using FirstProjectAPI.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Statistiques globales de la plateforme (réservé Admin).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _service.GetStatsAsync();
            return Ok(stats);
        }
    }
}