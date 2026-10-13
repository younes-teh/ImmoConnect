using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Services
{
    public interface ITokenService
    {
        // Prend l'utilisateur et ses rôles, renvoie le token JWT sous forme de texte
        string CreateToken(ApplicationUser user, List<string> roles);
    }
}