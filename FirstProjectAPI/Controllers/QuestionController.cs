using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstProjectAPI.Models;
using FirstProjectAPI.Services;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IServices _service;

        public QuestionController(IServices service)
        {
            _service = service;
        }

        // GET api/question/modalite/7
        [HttpGet("modalite/{idModalite}")]
        public ActionResult<List<Question>> GetByModalite(int idModalite)
        {
            var questions = _service.GetQuestionsByModalite(idModalite);
            return Ok(questions ?? new List<Question>());
        }

        // GET api/question/12
        [HttpGet("{id}")]
        public ActionResult<Question> GetById(int id)
        {
            var question = _service.GetQuestionById(id);
            if (question == null) return NotFound($"Question avec l'ID {id} introuvable.");
            return Ok(question);
        }

        // POST api/question/modalite/7
        [Authorize(Roles = "Admin,Formateur")]
        [HttpPost("modalite/{idModalite}")]
        public ActionResult Add(int idModalite, [FromBody] Question value)
        {
            if (value == null) return BadRequest("Données invalides.");
            _service.AddQuestion(idModalite, value);
            return StatusCode(201);
        }

        // PUT api/question/12
        [Authorize(Roles = "Admin,Formateur")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Question value)
        {
            if (value == null) return BadRequest("Données invalides.");

            try
            {
                _service.UpdateQuestion(id, value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE api/question/12
        [Authorize(Roles = "Admin,Formateur")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.DeleteQuestionById(id);
            return NoContent();
        }

        // POST api/question/12/repondre
        // Un apprenant connecté soumet sa réponse ; la correction est automatique.
        [Authorize(Roles = "Apprenant")]
        [HttpPost("{id}/repondre")]
        public ActionResult<ReponseApprenant> Repondre(int id, [FromBody] string reponse)
        {
            if (string.IsNullOrWhiteSpace(reponse)) return BadRequest("La réponse est vide.");

            var idUserClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!int.TryParse(idUserClaim, out var idUser))
                return Unauthorized("Utilisateur non identifiable depuis le token.");

            try
            {
                var result = _service.AddReponse(id, idUser, reponse);
                return StatusCode(201, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
