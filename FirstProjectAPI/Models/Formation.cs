using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    [Table("Formations")]
    public class Formation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public int? FormateurId { get; set; }

        public virtual Formateur? Formateur { get; set; }
        [Required]

        public int IdCategorie { get; set; }

        [ForeignKey("IdCategorie")]
        public virtual Categorie? Categorie { get; set; }

        [JsonIgnore]
        public virtual ICollection<Sessionf> Sessions { get; set; } = new List<Sessionf>();

        [JsonIgnore]
        public virtual ICollection<Modulef> Modules { get; set; } = new List<Modulef>();
    }
}