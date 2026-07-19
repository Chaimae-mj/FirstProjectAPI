using FirstProjectAPI.Dtos.ReponseApprenant;
using FirstProjectAPI.Extensions;
using FirstProjectAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReponseApprenantController : ControllerBase
    {
        private readonly IReponseApprenantService _service;

        public ReponseApprenantController(IReponseApprenantService service)
        {
            _service = service;
        }

        [HttpPost("repondre")]
        [Authorize(Roles = "Apprenant")]
        public IActionResult Repondre([FromBody] RepondreDto dto)
        {
            try
            {
                var userId = User.GetUserId();
                var resultat = _service.Repondre(userId, dto);
                return Ok(resultat);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("resultat/{idModalite}")]
        [Authorize(Roles = "Apprenant,Formateur,Admin")]
        public IActionResult Resultat(int idModalite)
        {
            try
            {
                var userId = User.GetUserId();
                var resultat = _service.GetResultat(userId, idModalite);
                return Ok(resultat);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}