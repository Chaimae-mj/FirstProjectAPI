namespace FirstProjectAPI.Dtos.Avatar
{
    // Ce DTO contient uniquement des informations de connexion "côté client"
    // (jeton LiveKit scopé à cette session) — jamais la clé API HeyGen elle-même.
    public class StartSessionResponseDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string LivekitUrl { get; set; } = string.Empty;

        public string LivekitClientToken { get; set; } = string.Empty;

        public int MaxSessionDuration { get; set; }
    }
}