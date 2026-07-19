using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.ReponseApprenant
{
    public class RepondreDto
    {
        [Required]
        public int IdQuestion { get; set; }

        [Required]
        public string ReponseDonnee { get; set; } = string.Empty;
    }
}