using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FirstProjectAPI.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [JsonIgnore]
        public string PasswordHash { get; set; }

        [Required]
        public Role Role { get; set; } = Role.Apprenant;

        [JsonIgnore]
        public virtual ICollection<Sessionf> Sessions { get; set; } = new List<Sessionf>();
    }
}