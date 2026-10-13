using Microsoft.AspNetCore.Identity;

namespace ImmoConnect.Api.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string NomComplet { get; set; } = string.Empty;
    }
}