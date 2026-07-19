using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    [Table("Modalites")]
    public class Modalite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titre { get; set; }

        [Required]
        public TypeModalite Type { get; set; }

        // Ordre d'affichage au sein du module.
        [Required]
        public int Ordre { get; set; }

        // Support de cours (théorie) ou énoncé (exercice). Optionnel pour un Examen
        // qui peut ne contenir que des Questions.
        public string? Contenu { get; set; }

        // Utile surtout pour un Examen chronométré.
        public int? DureeMinutes { get; set; }

        [Required]
        public int IdModule { get; set; }

        [ForeignKey("IdModule")]
        [JsonIgnore]
        public virtual Modulef? Module { get; set; }

        [JsonIgnore]
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
