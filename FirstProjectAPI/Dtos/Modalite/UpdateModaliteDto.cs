using System.ComponentModel.DataAnnotations;
using FirstProjectAPI.Models;

namespace FirstProjectAPI.Dtos.Modalite
{
    public class UpdateModaliteDto
    {
        [Required]
        [StringLength(150)]
        public string Titre { get; set; } = string.Empty;

        public string? Contenu { get; set; }

        [Required]
        public TypeModalite Type { get; set; }

        [Required]
        public int Ordre { get; set; }

        [Required]
        public int IdModule { get; set; }
    }
}