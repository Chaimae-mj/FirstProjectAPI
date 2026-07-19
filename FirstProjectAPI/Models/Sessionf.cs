using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    [Table("Sessions")]
    public class Sessionf
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public int Duree { get; set; }

        [Required]
        public int IdFormation { get; set; }

        [ForeignKey("IdFormation")]
        public virtual Formation? Formation { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}