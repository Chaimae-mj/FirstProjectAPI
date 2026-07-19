using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstProjectAPI.Models;
using FirstProjectAPI.Services;
using System.Collections.Generic;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormationController : ControllerBase
    {
        private readonly IServices _service;

        public FormationController(IServices service)
        {
            _service = service;
        }

        // 1. LIST
        [HttpGet]
        public ActionResult<List<Formation>> Get()
        {
            var formations = _service.GetAllFormations();
            return Ok(formations ?? new List<Formation>());
        }

        // 2. SEARCH (Utilise ta méthode GetFormationByName)
        [HttpGet("search/{nn}")]
        public ActionResult<List<Formation>> Search(string nn)
        {
            if (string.IsNullOrEmpty(nn)) return BadRequest("Le nom de recherche est vide.");
            var result = _service.GetFormationByName(nn);
            return Ok(result ?? new List<Formation>());
        }

        // 3. ADD
        [Authorize(Roles = "Admin,Formateur")]
        [HttpPost("{idcat}")]
        public ActionResult Add([FromRoute] int idcat, [FromBody] Formation value)
        {
            if (value == null) return BadRequest("Données invalides.");
            _service.AddFormation(idcat, value);
            return StatusCode(201);
        }

        // 4. UPDATE
        [Authorize(Roles = "Admin,Formateur")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Formation value)
        {
            if (value == null) return BadRequest("Données invalides.");

            try
            {
                _service.UpdateFormation(id, value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 5. DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.DeleteFormationById(id);
            return NoContent();
        }
    }
}