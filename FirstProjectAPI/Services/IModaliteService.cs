using FirstProjectAPI.Models;

namespace FirstProjectAPI.Services
{
    public interface IModaliteService
    {
        List<Modalite> GetByModule(int idModule);

        Modalite? GetById(int id);

        Modalite Create(int idModule, Modalite modalite);

        Modalite Update(int id, Modalite modalite);

        void Delete(int id);
    }
}