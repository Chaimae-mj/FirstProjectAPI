using FirstProjectAPI.Dtos.Ai;
using FirstProjectAPI.Services.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectAPI.Controllers
{
    [Route("api/ai")]
    [ApiController]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _service;

        public AiController(IAiService service)
        {
            _service = service;
        }

        /// <summary>
        /// L'apprenant pose une question libre au formateur IA.
        /// </summary>
        [Authorize(Roles = "Apprenant")]
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskDto dto)
        {
            try
            {
                var reponse = await _service.AskAsync(dto.Question);
                return Ok(new AskResponseDto { Reponse = reponse });
            }
            catch (GeminiApiException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
        }

        /// <summary>
        /// L'IA explique le contenu d'un module.
        /// </summary>
        [Authorize(Roles = "Apprenant")]
        [HttpPost("explainModule")]
        public async Task<IActionResult> ExplainModule([FromBody] ExplainModuleDto dto)
        {
            try
            {
                var result = await _service.ExplainModuleAsync(dto);
                return Ok(result);
            }
            catch (ModuleIntrouvableException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (GeminiApiException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
        }
    }
}