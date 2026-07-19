using System.ComponentModel.DataAnnotations;
using FirstProjectAPI.Models;

namespace FirstProjectAPI.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string Password { get; set; }

        // Optionnel : si absent, le rôle par défaut est Apprenant.
        // Seul un Admin devrait pouvoir créer un Formateur/Admin (voir AuthController).
        public Role? Role { get; set; }
    }
}
