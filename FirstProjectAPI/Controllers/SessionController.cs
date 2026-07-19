using FirstProjectAPI.Dtos.Session;
using FirstProjectAPI.Extensions;
using FirstProjectAPI.Services.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _service;

        public SessionController(ISessionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Liste de toutes les sessions (visible par tout utilisateur connecté).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var session = await _service.GetByIdAsync(id);

            if (session == null)
                return NotFound(new { message = "Session introuvable." });

            return Ok(session);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSessionDto dto)
        {
            try
            {
                var session = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
            }
            catch (FormationIntrouvableException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSessionDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "Session introuvable." });

                return NoContent();
            }
            catch (FormationIntrouvableException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Session introuvable." });

            return NoContent();
        }

        /// <summary>
        /// L'apprenant connecté s'inscrit à une session.
        /// </summary>
        [Authorize(Roles = "Apprenant")]
        [HttpPost("{id:int}/inscription")]
        public async Task<IActionResult> Inscrire(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var session = await _service.InscrireAsync(userId, id);
                return Ok(session);
            }
            catch (SessionNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DejaInscritException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Liste des apprenants inscrits à une session (Admin / Formateur).
        /// </summary>
        [Authorize(Roles = "Admin,Formateur")]
        [HttpGet("{id:int}/participants")]
        public async Task<IActionResult> GetParticipants(int id)
        {
            try
            {
                var participants = await _service.GetParticipantsAsync(id);
                return Ok(participants);
            }
            catch (SessionNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sessions auxquelles l'apprenant connecté est inscrit.
        /// </summary>
        [Authorize(Roles = "Apprenant")]
        [HttpGet("mes-sessions")]
        public async Task<IActionResult> GetMesSessions()
        {
            try
            {
                var userId = User.GetUserId();
                var sessions = await _service.GetMesSessionsAsync(userId);
                return Ok(sessions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}