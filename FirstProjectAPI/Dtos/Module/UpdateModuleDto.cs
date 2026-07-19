using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.Module
{
    public class UpdateModuleDto
    {
        [Required]
        [StringLength(150)]
        public string Titre { get; set; } = string.Empty;

        [Required]
        public int Ordre { get; set; }

        [Required]
        public int IdFormation { get; set; }
    }
}