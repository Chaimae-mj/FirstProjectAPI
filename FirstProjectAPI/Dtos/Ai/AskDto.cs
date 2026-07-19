using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.Ai
{
    public class AskDto
    {
        [Required]
        [StringLength(2000, MinimumLength = 3, ErrorMessage = "La question doit contenir entre 3 et 2000 caractères.")]
        public string Question { get; set; } = string.Empty;
    }
}