using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstProjectAPI.Models;
using FirstProjectAPI.Services;
using System.Collections.Generic;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly IServices _service;

        public CategorieController(IServices service)
        {
            _service = service;
        }

        // 1. LIST
        [HttpGet]
        public ActionResult<List<Categorie>> Index()
        {
            var categories = _service.GetAll();
            return Ok(categories ?? new List<Categorie>());
        }

        // 2. SEARCH (Par ID ou par logique interne, ici on utilise GetCatById)
        [HttpGet("{id}")]
        public ActionResult<Categorie> Search(int id)
        {
            var categorie = _service.GetCatById(id);
            if (categorie == null) return NotFound($"Catégorie avec l'ID {id} introuvable.");
            return Ok(categorie);
        }

        // 3. ADD
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Add([FromBody] Categorie value)
        {
            if (value == null) return BadRequest("Données invalides.");
            _service.AddCat(value);
            return StatusCode(201);
        }

        // 4. UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Categorie value)
        {
            if (value == null) return BadRequest("Données invalides.");

            try
            {
                _service.UpdateCat(id, value);
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
            _service.DeleteCatById(id);
            return NoContent();
        }
    }
}