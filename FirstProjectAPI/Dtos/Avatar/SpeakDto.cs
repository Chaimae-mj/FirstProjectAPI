using System.ComponentModel.DataAnnotations;

namespace FirstProjectAPI.Dtos.Avatar
{
    public class SpeakDto
    {
        [Required]
        [StringLength(2000, MinimumLength = 3)]
        public string Question { get; set; } = string.Empty;
    }
}