using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    [Table("ReponsesApprenants")]
    public class ReponseApprenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int IdQuestion { get; set; }

        [ForeignKey("IdQuestion")]
        [JsonIgnore]
        public virtual Question? Question { get; set; }

        [Required]
        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        [JsonIgnore]
        public virtual User? User { get; set; }

        [Required]
        [StringLength(255)]
        public string ReponseDonnee { get; set; }

        [Required]
        public bool EstCorrecte { get; set; }

        [Required]
        public DateTime DateReponse { get; set; } = DateTime.UtcNow;
    }
}
