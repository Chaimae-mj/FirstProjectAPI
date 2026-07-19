using FirstProjectAPI.Infra;
using FirstProjectAPI.Models;

namespace FirstProjectAPI.Services
{
    public class VService : IServices
    {
        private readonly IDao _dao;

        public VService(IDao dao)
        {
            _dao = dao;
        }

        public void AddCat(Categorie entity) => _dao.AddCat(entity);
        public void UpdateCat(int id, Categorie entity) => _dao.UpdateCat(id, entity);
        public void DeleteCatById(int id) => _dao.DeleteCatById(id);
        public List<Categorie> GetAll() => _dao.GetAll();
        public Categorie GetCatById(int id) => _dao.GetCatById(id);

        public void AddFormation(int idcat, Formation entity) => _dao.AddFormation(idcat, entity);
        public void UpdateFormation(int id, Formation entity) => _dao.UpdateFormation(id, entity);
        public void DeleteFormationById(int id) => _dao.DeleteFormationById(id);
        public List<Formation> GetAllFormations() => _dao.GetAllFormations();
        public Formation GetFormationById(int id) => _dao.GetFormationById(id);
        public List<Formation> GetFormationByName(string name) => _dao.GetFormationByName(name);

        public void AddSession(int idinformation, Sessionf entity) => _dao.AddSession(idinformation, entity);
        public void UpdateSession(int id, Sessionf entity) => _dao.UpdateSession(id, entity);
        public void DeleteSessionById(int id) => _dao.DeleteSessionById(id);
        public Sessionf GetSessionById(int id) => _dao.GetSessionById(id);
        public List<Sessionf> GetAllSessions() => _dao.GetAllSessions();
        public List<Sessionf> GetAllSessions(int idinformation) => _dao.GetAllSessions(idinformation);
        public List<Sessionf> GetAllSessions(DateTime debut, DateTime fin) => _dao.GetAllSessions(debut, fin);

        public void AddInscription(int idsession, int iduser) => _dao.AddInscription(idsession, iduser);
        public void DeleteInscription(int idsession, int iduser) => _dao.DeleteInscription(idsession, iduser);
        public List<User> GetAllUsersInSession(int idsession) => _dao.GetAllUsersInSession(idsession);
        public List<Sessionf> GetAllSessionsForUser(int iduser) => _dao.GetAllSessionsForUser(iduser);
        public void Add(User entity) => _dao.Add(entity);
        public User? GetUserByEmail(string email) => _dao.GetUserByEmail(email);
        public User? GetUserById(int id) => _dao.GetUserById(id);
        public List<User> GetAllUsers() => _dao.GetAllUsers();

        // --- Modules ---
        public void AddModule(int idFormation, Modulef entity) => _dao.AddModule(idFormation, entity);
        public void UpdateModule(int id, Modulef entity) => _dao.UpdateModule(id, entity);
        public void DeleteModuleById(int id) => _dao.DeleteModuleById(id);
        public Modulef GetModuleById(int id) => _dao.GetModuleById(id);
        public List<Modulef> GetModulesByFormation(int idFormation) => _dao.GetModulesByFormation(idFormation);

        // --- Modalites ---
        public void AddModalite(int idModule, Modalite entity) => _dao.AddModalite(idModule, entity);
        public void UpdateModalite(int id, Modalite entity) => _dao.UpdateModalite(id, entity);
        public void DeleteModaliteById(int id) => _dao.DeleteModaliteById(id);
        public Modalite GetModaliteById(int id) => _dao.GetModaliteById(id);
        public List<Modalite> GetModalitesByModule(int idModule) => _dao.GetModalitesByModule(idModule);

        // --- Questions ---
        public void AddQuestion(int idModalite, Question entity) => _dao.AddQuestion(idModalite, entity);
        public void UpdateQuestion(int id, Question entity) => _dao.UpdateQuestion(id, entity);
        public void DeleteQuestionById(int id) => _dao.DeleteQuestionById(id);
        public Question GetQuestionById(int id) => _dao.GetQuestionById(id);
        public List<Question> GetQuestionsByModalite(int idModalite) => _dao.GetQuestionsByModalite(idModalite);

        // --- Réponses des apprenants ---
        public ReponseApprenant AddReponse(int idQuestion, int idUser, string reponseDonnee) =>
            _dao.AddReponse(idQuestion, idUser, reponseDonnee);
        public List<ReponseApprenant> GetReponsesByUser(int idUser) => _dao.GetReponsesByUser(idUser);
        public List<ReponseApprenant> GetReponsesByQuestion(int idQuestion) => _dao.GetReponsesByQuestion(idQuestion);
    }
}