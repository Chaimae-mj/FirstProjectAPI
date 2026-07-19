using FirstProjectAPI.Dtos.Avatar;
using FirstProjectAPI.Extensions;
using FirstProjectAPI.Services.Avatar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectAPI.Controllers
{
    [Route("api/avatar")]
    [ApiController]
    [Authorize(Roles = "Apprenant")]
    public class AvatarController : ControllerBase
    {
        private readonly IAvatarService _service;

        public AvatarController(IAvatarService service)
        {
            _service = service;
        }

        /// <summary>
        /// Démarre une session avatar. Renvoie les infos LiveKit à utiliser côté
        /// frontend (SDK HeyGen LiveAvatar) pour afficher la vidéo de l'avatar.
        /// </summary>
        [HttpPost("session/start")]
        public async Task<IActionResult> StartSession()
        {
            try
            {
                var userId = User.GetUserId();
                var result = await _service.StartSessionAsync(userId);
                return Ok(result);
            }
            catch (AvatarSessionAlreadyActiveException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (HeyGenApiException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Envoie une question : Gemini génère la réponse, l'avatar la prononce.
        /// </summary>
        [HttpPost("speak")]
        public async Task<IActionResult> Speak([FromBody] SpeakDto dto)
        {
            try
            {
                var userId = User.GetUserId();
                var reponse = await _service.SpeakAsync(userId, dto.Question);
                return Ok(new SpeakResponseDto { ReponseTexte = reponse });
            }
            catch (AvatarSessionNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (HeyGenApiException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
            catch (TtsApiException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Arrête la session avatar en cours.
        /// </summary>
        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            try
            {
                var userId = User.GetUserId();
                await _service.StopAsync(userId);
                return NoContent();
            }
            catch (AvatarSessionNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}