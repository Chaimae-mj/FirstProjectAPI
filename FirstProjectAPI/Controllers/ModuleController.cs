using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstProjectAPI.Models;
using FirstProjectAPI.Services;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IServices _service;

        public ModuleController(IServices service)
        {
            _service = service;
        }

        // GET api/module/formation/3
        // Liste des modules d'une formation, triés par Ordre.
        [HttpGet("formation/{idFormation}")]
        public ActionResult<List<Modulef>> GetByFormation(int idFormation)
        {
            var modules = _service.GetModulesByFormation(idFormation);
            return Ok(modules ?? new List<Modulef>());
        }

        // GET api/module/5
        [HttpGet("{id}")]
        public ActionResult<Modulef> GetById(int id)
        {
            var module = _service.GetModuleById(id);
            if (module == null) return NotFound($"Module avec l'ID {id} introuvable.");
            return Ok(module);
        }

        // POST api/module/formation/3
        [Authorize(Roles = "Admin,Formateur")]
        [HttpPost("formation/{idFormation}")]
        public ActionResult Add(int idFormation, [FromBody] Modulef value)
        {
            if (value == null) return BadRequest("Données invalides.");
            _service.AddModule(idFormation, value);
            return StatusCode(201);
        }

        // PUT api/module/5
        [Authorize(Roles = "Admin,Formateur")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Modulef value)
        {
            if (value == null) return BadRequest("Données invalides.");

            try
            {
                _service.UpdateModule(id, value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE api/module/5
        [Authorize(Roles = "Admin,Formateur")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.DeleteModuleById(id);
            return NoContent();
        }
    }
}
