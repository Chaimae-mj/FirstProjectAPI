namespace FirstProjectAPI.Dtos.Avatar
{
    public class SpeakResponseDto
    {
        // Texte généré par Gemini, envoyé à l'avatar sous forme audio.
        // Utile pour afficher un sous-titre côté frontend en parallèle de la vidéo.
        public string ReponseTexte { get; set; } = string.Empty;
    }
}