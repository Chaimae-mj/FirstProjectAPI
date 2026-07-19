using FirstProjectAPI.Dtos.Session;
using FirstProjectAPI.Infra;
using FirstProjectAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectAPI.Services.Session
{
    public class SessionService : ISessionService
    {
        private readonly MyContext _context;

        public SessionService(MyContext context)
        {
            _context = context;
        }

        public async Task<List<SessionDto>> GetAllAsync()
        {
            var sessions = await _context.Sessions
                .Include(s => s.Formation)
                .Include(s => s.Users)
                .ToListAsync();

            return sessions.Select(ToDto).ToList();
        }

        public async Task<SessionDto?> GetByIdAsync(int id)
        {
            var session = await _context.Sessions
                .Include(s => s.Formation)
                .Include(s => s.Users)
                .FirstOrDefaultAsync(s => s.Id == id);

            return session == null ? null : ToDto(session);
        }

        public async Task<SessionDto> CreateAsync(CreateSessionDto dto)
        {
            var formationExiste = await _context.Formations.AnyAsync(f => f.Id == dto.IdFormation);
            if (!formationExiste)
                throw new FormationIntrouvableException();

            var session = new Sessionf
            {
                DateDebut = dto.DateDebut,
                Duree = dto.Duree,
                IdFormation = dto.IdFormation
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            // Recharger avec la formation pour le mapping du DTO
            await _context.Entry(session).Reference(s => s.Formation).LoadAsync();

            return ToDto(session);
        }

        public async Task<bool> UpdateAsync(int id, UpdateSessionDto dto)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
                return false;

            var formationExiste = await _context.Formations.AnyAsync(f => f.Id == dto.IdFormation);
            if (!formationExiste)
                throw new FormationIntrouvableException();

            session.DateDebut = dto.DateDebut;
            session.Duree = dto.Duree;
            session.IdFormation = dto.IdFormation;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
                return false;

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<SessionDto> InscrireAsync(int userId, int idSession)
        {
            var session = await _context.Sessions
                .Include(s => s.Formation)
                .Include(s => s.Users)
                .FirstOrDefaultAsync(s => s.Id == idSession);

            if (session == null)
                throw new SessionNotFoundException();

            if (session.Users.Any(u => u.Id == userId))
                throw new DejaInscritException();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur introuvable.");

            session.Users.Add(user);
            await _context.SaveChangesAsync();

            return ToDto(session);
        }

        public async Task<List<ParticipantDto>> GetParticipantsAsync(int idSession)
        {
            var session = await _context.Sessions
                .Include(s => s.Users)
                .FirstOrDefaultAsync(s => s.Id == idSession);

            if (session == null)
                throw new SessionNotFoundException();

            return session.Users
                .Select(u => new ParticipantDto
                {
                    IdUser = u.Id,
                    Nom = u.Name,
                    Email = u.Email
                })
                .ToList();
        }

        public async Task<List<SessionDto>> GetMesSessionsAsync(int userId)
        {
            var sessions = await _context.Sessions
                .Include(s => s.Formation)
                .Include(s => s.Users)
                .Where(s => s.Users.Any(u => u.Id == userId))
                .ToListAsync();

            return sessions.Select(ToDto).ToList();
        }

        private static SessionDto ToDto(Sessionf s)
        {
            return new SessionDto
            {
                Id = s.Id,
                DateDebut = s.DateDebut,
                Duree = s.Duree,
                DateFin = s.DateDebut.AddDays(s.Duree),
                IdFormation = s.IdFormation,
                FormationNom = s.Formation?.Name,
                NombreInscrits = s.Users?.Count ?? 0
            };
        }
    }
}