namespace ImmoConnect.Api.Models.DTO
{
    // Ce que l'API renvoie au client après un register / login réussi
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;       // le JWT à renvoyer dans l'en-tête Authorization
        public string Email { get; set; } = string.Empty;
        public string NomComplet { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();        // utile au front React pour afficher/masquer des boutons
    }
}