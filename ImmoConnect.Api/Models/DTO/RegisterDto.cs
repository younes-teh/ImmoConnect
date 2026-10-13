using System.ComponentModel.DataAnnotations;

namespace ImmoConnect.Api.Models.DTO
{
    public class RegisterDto
    {
        // Nom complet de l'utilisateur (obligatoire, 100 caractères max)
        [Required(ErrorMessage = "Le nom complet est obligatoire")]
        [MaxLength(100)]
        public string NomComplet { get; set; } = string.Empty;

        // Email : sert aussi d'identifiant de connexion
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        // Mot de passe en clair (jamais stocké tel quel : Identity le hashe)
        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [MinLength(6, ErrorMessage = "6 caractères minimum")]
        public string Password { get; set; } = string.Empty;
    }
}