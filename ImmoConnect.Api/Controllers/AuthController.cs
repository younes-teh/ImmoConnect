using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;
using ImmoConnect.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImmoConnect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // UserManager : créer un utilisateur, chercher par email, vérifier un mot de passe, gérer ses rôles
        private readonly UserManager<ApplicationUser> userManager;
        // RoleManager : créer / vérifier l'existence des rôles (Admin, Agent)
        private readonly RoleManager<IdentityRole> roleManager;
        // Notre service qui fabrique le JWT
        private readonly ITokenService tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Vérifier que l'email n'est pas déjà pris -> 409 Conflict
            var existing = await userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict("Cet email est déjà utilisé");

            // 2. Construire l'utilisateur (UserName = Email pour simplifier)
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                NomComplet = dto.NomComplet
            };

            // 3. Création : Identity hashe le mot de passe et applique les règles de complexité
            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                // On renvoie la liste des erreurs (ex : "Passwords must have at least one digit")
                return BadRequest(result.Errors.Select(e => e.Description));

            // 4. Rôle par défaut = Agent. On crée le rôle s'il n'existe pas encore
            if (!await roleManager.RoleExistsAsync("Agent"))
                await roleManager.CreateAsync(new IdentityRole("Agent"));

            await userManager.AddToRoleAsync(user, "Agent");

            // 5. Connexion automatique : on renvoie directement un token
            var roles = (await userManager.GetRolesAsync(user)).ToList();
            return Ok(BuildResponse(user, roles));
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Chercher l'utilisateur par email
            var user = await userManager.FindByEmailAsync(dto.Email);

            // 2. Vérifier le mot de passe (comparaison avec le hash)
            //    Même message pour "email inconnu" et "mauvais mot de passe" :
            //    on ne dit pas à un attaquant quel email existe
            if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Email ou mot de passe incorrect");

            // 3. Récupérer ses rôles (IList -> List) et générer le token
            var roles = (await userManager.GetRolesAsync(user)).ToList();
            return Ok(BuildResponse(user, roles));
        }

        // Méthode privée : évite de dupliquer la construction de la réponse
        private AuthResponseDto BuildResponse(ApplicationUser user, List<string> roles)
        {
            return new AuthResponseDto
            {
                Token = tokenService.CreateToken(user, roles),
                Email = user.Email ?? string.Empty,
                NomComplet = user.NomComplet,
                Roles = roles
            };
        }
    }
}