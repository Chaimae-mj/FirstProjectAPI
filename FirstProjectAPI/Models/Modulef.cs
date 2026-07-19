using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    // Nommée "Modulef" (comme "Sessionf") pour éviter tout conflit avec System.Reflection.Module.
    [Table("Modules")]
    public class Modulef
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titre { get; set; }

        // Permet d'afficher les modules dans le bon ordre au sein d'une formation.
        [Required]
        public int Ordre { get; set; }

        [Required]
        public int IdFormation { get; set; }

        [ForeignKey("IdFormation")]
        [JsonIgnore]
        public virtual Formation? Formation { get; set; }

        [JsonIgnore]
        public virtual ICollection<Modalite> Modalites { get; set; } = new List<Modalite>();
    }
}
