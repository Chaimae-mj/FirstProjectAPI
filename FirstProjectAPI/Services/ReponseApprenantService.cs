using FirstProjectAPI.Dtos.ReponseApprenant;
using FirstProjectAPI.Infra;
using FirstProjectAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectAPI.Services
{
    public class ReponseApprenantService : IReponseApprenantService
    {
        private readonly MyContext _context;

        public ReponseApprenantService(MyContext context)
        {
            _context = context;
        }

        public ResultatDto Repondre(int userId, RepondreDto dto)
        {
            var question = _context.Questions
                .FirstOrDefault(q => q.Id == dto.IdQuestion);

            if (question == null)
                throw new KeyNotFoundException("Question introuvable.");

            bool estCorrecte =
                question.ReponseCorrecte.Trim().ToLower() ==
                dto.ReponseDonnee.Trim().ToLower();

            var reponse = new ReponseApprenant
            {
                IdQuestion = dto.IdQuestion,
                IdUser = userId,
                ReponseDonnee = dto.ReponseDonnee,
                EstCorrecte = estCorrecte,
                DateReponse = DateTime.UtcNow
            };

            _context.ReponsesApprenants.Add(reponse);
            _context.SaveChanges();

            return GetResultat(userId, question.IdModalite);
        }

        public ResultatDto GetResultat(int userId, int idModalite)
        {
            var reponses = _context.ReponsesApprenants
                .Include(r => r.Question)
                .Where(r =>
                    r.IdUser == userId &&
                    r.Question != null &&
                    r.Question.IdModalite == idModalite)
                .ToList();

            int total = reponses.Count;

            int bonnes = reponses.Count(r => r.EstCorrecte);

            return new ResultatDto
            {
                TotalQuestions = total,
                BonnesReponses = bonnes,
                MauvaisesReponses = total - bonnes,
                Pourcentage = total == 0
                    ? 0
                    : Math.Round((double)bonnes / total * 100, 2)
            };
        }
    }
}