using FirstProjectAPI.Infra;
using FirstProjectAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectAPI.Services
{
    public class ModaliteService : IModaliteService
    {
        private readonly MyContext _context;

        public ModaliteService(MyContext context)
        {
            _context = context;
        }

        public List<Modalite> GetByModule(int idModule)
        {
            return _context.Modalites
                .Where(m => m.IdModule == idModule)
                .OrderBy(m => m.Ordre)
                .ToList();
        }

        public Modalite? GetById(int id)
        {
            return _context.Modalites.FirstOrDefault(x => x.Id == id);
        }

        public Modalite Create(int idModule, Modalite modalite)
        {
            var module = _context.Modules.FirstOrDefault(x => x.Id == idModule);

            if (module == null)
                throw new Exception("Module introuvable.");

            modalite.IdModule = idModule;

            _context.Modalites.Add(modalite);
            _context.SaveChanges();

            return modalite;
        }

        public Modalite Update(int id, Modalite modalite)
        {
            var entity = _context.Modalites.FirstOrDefault(x => x.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Modalité introuvable.");

            entity.Titre = modalite.Titre;
            entity.Type = modalite.Type;
            entity.Ordre = modalite.Ordre;
            entity.Contenu = modalite.Contenu;
            entity.DureeMinutes = modalite.DureeMinutes;

            _context.SaveChanges();

            return entity;
        }

        public void Delete(int id)
        {
            var entity = _context.Modalites.FirstOrDefault(x => x.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Modalité introuvable.");

            _context.Modalites.Remove(entity);
            _context.SaveChanges();
        }
    }
}