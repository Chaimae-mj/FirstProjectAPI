using FirstProjectAPI.Models;

namespace FirstProjectAPI.Services
{
    public interface IServices
    {
        void AddCat(Categorie entity);
        void UpdateCat(int id, Categorie entity);
        void DeleteCatById(int id);
        List<Categorie> GetAll();
        Categorie GetCatById(int id);

        void AddFormation(int idcat, Formation entity);
        void UpdateFormation(int id, Formation entity);
        void DeleteFormationById(int id);
        List<Formation> GetAllFormations();
        Formation GetFormationById(int id);
        List<Formation> GetFormationByName(string name);

        void AddSession(int idinformation, Sessionf entity);
        void UpdateSession(int id, Sessionf entity);
        void DeleteSessionById(int id);
        Sessionf GetSessionById(int id);
        List<Sessionf> GetAllSessions();
        List<Sessionf> GetAllSessions(int idinformation);
        List<Sessionf> GetAllSessions(DateTime debut, DateTime fin);

        void AddInscription(int idsession, int iduser);
        void DeleteInscription(int idsession, int iduser);
        List<User> GetAllUsersInSession(int idsession);
        List<Sessionf> GetAllSessionsForUser(int iduser);
        void Add(User entity);
        User? GetUserByEmail(string email);
        User? GetUserById(int id);
        List<User> GetAllUsers();

        // --- Modules ---
        void AddModule(int idFormation, Modulef entity);
        void UpdateModule(int id, Modulef entity);
        void DeleteModuleById(int id);
        Modulef GetModuleById(int id);
        List<Modulef> GetModulesByFormation(int idFormation);

        // --- Modalites ---
        void AddModalite(int idModule, Modalite entity);
        void UpdateModalite(int id, Modalite entity);
        void DeleteModaliteById(int id);
        Modalite GetModaliteById(int id);
        List<Modalite> GetModalitesByModule(int idModule);

        // --- Questions ---
        void AddQuestion(int idModalite, Question entity);
        void UpdateQuestion(int id, Question entity);
        void DeleteQuestionById(int id);
        Question GetQuestionById(int id);
        List<Question> GetQuestionsByModalite(int idModalite);

        // --- Réponses des apprenants ---
        ReponseApprenant AddReponse(int idQuestion, int idUser, string reponseDonnee);
        List<ReponseApprenant> GetReponsesByUser(int idUser);
        List<ReponseApprenant> GetReponsesByQuestion(int idQuestion);
    }
}