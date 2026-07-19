using Microsoft.EntityFrameworkCore;
using FirstProjectAPI.Models;

namespace FirstProjectAPI.Infra
{
    public class DaoImpl : IDao
    {
        private readonly MyContext _context;

        public DaoImpl(MyContext context)
        {
            _context = context;
        }

        public void AddCat(Categorie entity)
        {
            _context.Categories.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateCat(int id, Categorie entity)
        {
            var existing = _context.Categories.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"Catégorie avec l'ID {id} introuvable.");

            existing.Name = entity.Name;
            _context.SaveChanges();
        }

        public void DeleteCatById(int id)
        {
            var cat = _context.Categories.Find(id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                _context.SaveChanges();
            }
        }

        public List<Categorie> GetAll() => _context.Categories.ToList();

        public Categorie GetCatById(int id) => _context.Categories.Find(id)!;

        public void AddFormation(int idcat, Formation entity)
        {
            entity.IdCategorie = idcat;
            _context.Formations.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateFormation(int id, Formation entity)
        {
            var existing = _context.Formations.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"Formation avec l'ID {id} introuvable.");

            existing.Name = entity.Name;

            // On ne change la catégorie que si une nouvelle valeur cohérente est fournie
            if (entity.IdCategorie > 0)
                existing.IdCategorie = entity.IdCategorie;

            _context.SaveChanges();
        }

        public void DeleteFormationById(int id)
        {
            var formation = _context.Formations.Find(id);
            if (formation != null)
            {
                _context.Formations.Remove(formation);
                _context.SaveChanges();
            }
        }

        public List<Formation> GetAllFormations() => _context.Formations.Include(f => f.Categorie).ToList();

        public Formation GetFormationById(int id) => _context.Formations.Include(f => f.Categorie).FirstOrDefault(f => f.Id == id)!;

        public List<Formation> GetFormationByName(string name) =>
            _context.Formations.Where(f => f.Name.Contains(name)).Include(f => f.Categorie).ToList();

        public void AddSession(int idinformation, Sessionf entity)
        {
            entity.IdFormation = idinformation;
            _context.Sessions.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateSession(int id, Sessionf entity)
        {
            var existing = _context.Sessions.Find(id);
            if (existing != null)
            {
                existing.DateDebut = entity.DateDebut;
                existing.Duree = entity.Duree;
                _context.SaveChanges();
            }
        }

        public void DeleteSessionById(int id)
        {
            var session = _context.Sessions.Find(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
                _context.SaveChanges();
            }
        }

        public Sessionf GetSessionById(int id) => _context.Sessions.Include(s => s.Formation).FirstOrDefault(s => s.Id == id)!;

        public List<Sessionf> GetAllSessions() => _context.Sessions.Include(s => s.Formation).ToList();

        public List<Sessionf> GetAllSessions(int idinformation) =>
            _context.Sessions.Where(s => s.IdFormation == idinformation).Include(s => s.Formation).ToList();

        public List<Sessionf> GetAllSessions(DateTime debut, DateTime fin) =>
            _context.Sessions.Where(s => s.DateDebut >= debut && s.DateDebut <= fin).Include(s => s.Formation).ToList();

        public void AddInscription(int idsession, int iduser)
        {
            var session = _context.Sessions.Include(s => s.Users).FirstOrDefault(s => s.Id == idsession);
            var user = _context.Users.Find(iduser);

            if (session != null && user != null)
            {
                session.Users.Add(user);
                _context.SaveChanges();
            }
        }

        public void DeleteInscription(int idsession, int iduser)
        {
            var session = _context.Sessions.Include(s => s.Users).FirstOrDefault(s => s.Id == idsession);
            var user = _context.Users.Find(iduser);

            if (session != null && user != null)
            {
                session.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public List<User> GetAllUsersInSession(int idsession)
        {
            var session = _context.Sessions.Include(s => s.Users).FirstOrDefault(s => s.Id == idsession);
            return session != null ? session.Users.ToList() : new List<User>();
        }

        public List<Sessionf> GetAllSessionsForUser(int iduser)
        {
            var user = _context.Users.Include(u => u.Sessions).FirstOrDefault(u => u.Id == iduser);
            return user != null ? user.Sessions.ToList() : new List<Sessionf>();
        }

        public void Add(User entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public User? GetUserByEmail(string email) =>
            _context.Users.FirstOrDefault(u => u.Email == email);

        public User? GetUserById(int id) => _context.Users.Find(id);

        public List<User> GetAllUsers() => _context.Users.ToList();

        // --- Modules ---

        public void AddModule(int idFormation, Modulef entity)
        {
            entity.IdFormation = idFormation;
            _context.Modules.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateModule(int id, Modulef entity)
        {
            var existing = _context.Modules.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"Module avec l'ID {id} introuvable.");

            existing.Titre = entity.Titre;
            existing.Ordre = entity.Ordre;
            _context.SaveChanges();
        }

        public void DeleteModuleById(int id)
        {
            var module = _context.Modules.Find(id);
            if (module != null)
            {
                _context.Modules.Remove(module);
                _context.SaveChanges();
            }
        }

        public Modulef GetModuleById(int id) =>
            _context.Modules.Include(m => m.Modalites).FirstOrDefault(m => m.Id == id)!;

        public List<Modulef> GetModulesByFormation(int idFormation) =>
            _context.Modules.Where(m => m.IdFormation == idFormation)
                .OrderBy(m => m.Ordre)
                .Include(m => m.Modalites)
                .ToList();

        // --- Modalites ---

        public void AddModalite(int idModule, Modalite entity)
        {
            entity.IdModule = idModule;
            _context.Modalites.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateModalite(int id, Modalite entity)
        {
            var existing = _context.Modalites.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"Modalité avec l'ID {id} introuvable.");

            existing.Titre = entity.Titre;
            existing.Type = entity.Type;
            existing.Ordre = entity.Ordre;
            existing.Contenu = entity.Contenu;
            existing.DureeMinutes = entity.DureeMinutes;
            _context.SaveChanges();
        }

        public void DeleteModaliteById(int id)
        {
            var modalite = _context.Modalites.Find(id);
            if (modalite != null)
            {
                _context.Modalites.Remove(modalite);
                _context.SaveChanges();
            }
        }

        public Modalite GetModaliteById(int id) =>
            _context.Modalites.Include(m => m.Questions).FirstOrDefault(m => m.Id == id)!;

        public List<Modalite> GetModalitesByModule(int idModule) =>
            _context.Modalites.Where(m => m.IdModule == idModule)
                .OrderBy(m => m.Ordre)
                .ToList();

        // --- Questions ---

        public void AddQuestion(int idModalite, Question entity)
        {
            entity.IdModalite = idModalite;
            _context.Questions.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateQuestion(int id, Question entity)
        {
            var existing = _context.Questions.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"Question avec l'ID {id} introuvable.");

            existing.Enonce = entity.Enonce;
            existing.Option1 = entity.Option1;
            existing.Option2 = entity.Option2;
            existing.Option3 = entity.Option3;
            existing.Option4 = entity.Option4;
            existing.ReponseCorrecte = entity.ReponseCorrecte;
            _context.SaveChanges();
        }

        public void DeleteQuestionById(int id)
        {
            var question = _context.Questions.Find(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }
        }

        public Question GetQuestionById(int id) => _context.Questions.Find(id)!;

        public List<Question> GetQuestionsByModalite(int idModalite) =>
            _context.Questions.Where(q => q.IdModalite == idModalite).ToList();

        // --- Réponses des apprenants ---

        public ReponseApprenant AddReponse(int idQuestion, int idUser, string reponseDonnee)
        {
            var question = _context.Questions.Find(idQuestion)
                ?? throw new KeyNotFoundException($"Question avec l'ID {idQuestion} introuvable.");

            var estCorrecte = string.Equals(
                question.ReponseCorrecte.Trim(),
                reponseDonnee.Trim(),
                StringComparison.OrdinalIgnoreCase);

            var reponse = new ReponseApprenant
            {
                IdQuestion = idQuestion,
                IdUser = idUser,
                ReponseDonnee = reponseDonnee,
                EstCorrecte = estCorrecte,
                DateReponse = DateTime.UtcNow
            };

            _context.ReponsesApprenants.Add(reponse);
            _context.SaveChanges();

            return reponse;
        }

        public List<ReponseApprenant> GetReponsesByUser(int idUser) =>
            _context.ReponsesApprenants.Include(r => r.Question)
                .Where(r => r.IdUser == idUser)
                .ToList();

        public List<ReponseApprenant> GetReponsesByQuestion(int idQuestion) =>
            _context.ReponsesApprenants.Where(r => r.IdQuestion == idQuestion).ToList();
    }
}