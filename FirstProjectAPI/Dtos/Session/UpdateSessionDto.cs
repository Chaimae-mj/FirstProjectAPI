using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.Session
{
    public class UpdateSessionDto
    {
        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "La durée doit être comprise entre 1 et 365 jours.")]
        public int Duree { get; set; }

        [Required]
        public int IdFormation { get; set; }
    }
}