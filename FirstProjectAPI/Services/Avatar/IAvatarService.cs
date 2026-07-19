using FirstProjectAPI.Dtos.Avatar;

namespace FirstProjectAPI.Services.Avatar
{
    public interface IAvatarService
    {
        /// <summary>
        /// Démarre une session avatar HeyGen LiveAvatar pour l'apprenant (userId).
        /// Ouvre en interne le WebSocket de contrôle et le garde ouvert côté serveur.
        /// </summary>
        Task<StartSessionResponseDto> StartSessionAsync(int userId);

        /// <summary>
        /// Envoie la question à Gemini, convertit la réponse en audio (TTS),
        /// puis l'envoie à l'avatar pour qu'il la prononce.
        /// </summary>
        Task<string> SpeakAsync(int userId, string question);

        /// <summary>
        /// Arrête la session avatar et ferme le WebSocket.
        /// </summary>
        Task StopAsync(int userId);
    }
}