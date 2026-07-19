using FirstProjectAPI.Dtos.Session;

namespace FirstProjectAPI.Services.Session
{
    public interface ISessionService
    {
        Task<List<SessionDto>> GetAllAsync();

        Task<SessionDto?> GetByIdAsync(int id);

        Task<SessionDto> CreateAsync(CreateSessionDto dto);

        Task<bool> UpdateAsync(int id, UpdateSessionDto dto);

        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// L'apprenant connecté (userId) s'inscrit à la session idSession.
        /// </summary>
        Task<SessionDto> InscrireAsync(int userId, int idSession);

        Task<List<ParticipantDto>> GetParticipantsAsync(int idSession);

        /// <summary>
        /// Liste des sessions auxquelles l'utilisateur (userId) est inscrit.
        /// </summary>
        Task<List<SessionDto>> GetMesSessionsAsync(int userId);
    }
}