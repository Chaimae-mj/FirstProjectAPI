using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.DTOs.Formateur
{
    public class UpdateFormateurDto
    {
        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Telephone { get; set; }

        public string? Specialite { get; set; }

        public string? Photo { get; set; }
    }
}