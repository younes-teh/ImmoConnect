using ImmoConnect.Api.Models.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImmoConnect.Api.Services   // corrigé : même namespace que ITokenService
{
    public class TokenService : ITokenService
    {
        // IConfiguration permet de lire les valeurs de appsettings.json
        private readonly IConfiguration configuration;

        // Le constructeur reçoit IConfiguration par injection de dépendances
        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateToken(ApplicationUser user, List<string> roles)
        {
            // 1. Les claims = les infos stockées dans le token (id, email, nom...)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),             // identifiant unique de l'utilisateur
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),   // email (chaîne vide si null)
                new Claim(ClaimTypes.Name, user.NomComplet)                // nom complet affiché
            };

            // 2. On ajoute un claim par rôle (Admin, Agent...)
            //    C'est ce que [Authorize(Roles = "Admin")] va lire pour autoriser ou refuser
            foreach (var role in roles)
            {
                // CORRIGÉ : claims.Add(...) est indispensable, sinon le claim est créé puis perdu
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. On transforme la clé secrète (texte) en bytes, puis en clé de sécurité
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            // 4. Credentials = la clé + l'algorithme de signature (HMAC SHA-256)
            //    La signature empêche quelqu'un de modifier le token sans que le serveur le détecte
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. On construit le token
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],        // qui a émis le token
                audience: configuration["Jwt:Audience"],    // pour qui il est destiné
                claims: claims,                             // les infos contenues dans le token
                expires: DateTime.UtcNow.AddMinutes(        // date d'expiration
                    double.Parse(configuration["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds);                 // la signature

            // 6. On convertit le token en texte "xxxxx.yyyyy.zzzzz" à renvoyer au client
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}