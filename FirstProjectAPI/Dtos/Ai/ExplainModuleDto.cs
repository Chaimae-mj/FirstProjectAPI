using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.Ai
{
    public class ExplainModuleDto
    {
        [Required]
        public int IdModule { get; set; }

        // Optionnel : question précise de l'apprenant sur ce module.
        // Si vide, l'IA fait une explication générale du module.
        [StringLength(1000)]
        public string? QuestionSpecifique { get; set; }
    }
}