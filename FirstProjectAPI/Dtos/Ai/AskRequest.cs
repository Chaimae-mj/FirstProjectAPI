using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.Ai
{
    public class AskRequest
    {
        [Required]
        public string Question { get; set; } = string.Empty;
    }
}