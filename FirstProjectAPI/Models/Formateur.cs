using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Models
{
    public class Formateur
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Telephone { get; set; }

        public string? Specialite { get; set; }

        public string? Photo { get; set; }

        // Un formateur peut enseigner plusieurs formations
        public ICollection<Formation>? Formations { get; set; }
    }
}