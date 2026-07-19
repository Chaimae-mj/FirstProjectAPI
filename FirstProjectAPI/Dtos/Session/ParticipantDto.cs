namespace FirstProjectAPI.Dtos.Session
{
    public class ParticipantDto
    {
        public int IdUser { get; set; }

        public string Nom { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}