using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    [Table("Questions")]
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Enonce { get; set; }

        // Options de réponse pour un QCM. Laisser vide si question ouverte.
        [StringLength(255)]
        public string? Option1 { get; set; }

        [StringLength(255)]
        public string? Option2 { get; set; }

        [StringLength(255)]
        public string? Option3 { get; set; }

        [StringLength(255)]
        public string? Option4 { get; set; }

        [Required]
        [StringLength(255)]
        public string ReponseCorrecte { get; set; }

        [Required]
        public int IdModalite { get; set; }

        [ForeignKey("IdModalite")]
        [JsonIgnore]
        public virtual Modalite? Modalite { get; set; }
    }
}
