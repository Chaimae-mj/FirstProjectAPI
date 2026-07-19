using FirstProjectAPI.DTOs.Formateur;
using FirstProjectAPI.Services.Formateur;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormateurController : ControllerBase
    {
        private readonly IFormateurService _service;

        public FormateurController(IFormateurService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var formateur = await _service.GetByIdAsync(id);

            if (formateur == null)
                return NotFound();

            return Ok(formateur);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateFormateurDto dto)
        {
            var formateur = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = formateur.Id }, formateur);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFormateurDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}