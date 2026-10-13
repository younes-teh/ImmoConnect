using ImmoConnect.Api.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace ImmoConnect.Api.Data
{
    // Classe statique : on l'appelle une seule fois au démarrage de l'API
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Créer les rôles s'ils n'existent pas encore
            string[] roles = { "Admin", "Agent" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Créer le compte Admin s'il n'existe pas encore
            //    (compte de démo : en production le mot de passe viendrait d'un secret, pas du code)
            const string adminEmail = "admin@immoconnect.com";
            const string adminPassword = "Admin123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomComplet = "Administrateur"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}