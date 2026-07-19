namespace FirstProjectAPI.DTOs.Formateur
{
    public class FormateurDto
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;

        public string Prenom { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Telephone { get; set; }

        public string? Specialite { get; set; }

        public string? Photo { get; set; }
    }
}