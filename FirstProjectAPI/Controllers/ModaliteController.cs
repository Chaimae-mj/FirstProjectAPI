using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstProjectAPI.Models;
using FirstProjectAPI.Services;

namespace FirstProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModaliteController : ControllerBase
    {
        private readonly IModaliteService _service;

        public ModaliteController(IModaliteService service)
        {
            _service = service;
        }

        [HttpGet("module/{idModule}")]
        public ActionResult<List<Modalite>> GetByModule(int idModule)
        {
            return Ok(_service.GetByModule(idModule));
        }

        [HttpGet("{id}")]
        public ActionResult<Modalite> GetById(int id)
        {
            var modalite = _service.GetById(id);

            if (modalite == null)
                return NotFound();

            return Ok(modalite);
        }


        [Authorize(Roles = "Admin,Formateur")]
        [HttpPost("module/{idModule}")]
        public ActionResult Create(int idModule, Modalite modalite)
        {
            var result = _service.Create(idModule, modalite);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin,Formateur")]
        [HttpPut("{id}")]
        public ActionResult Update(int id, Modalite modalite)
        {
            _service.Update(id, modalite);

            return NoContent();
        }

        [Authorize(Roles = "Admin,Formateur")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _service.Delete(id);

            return NoContent();
        }
    }
}