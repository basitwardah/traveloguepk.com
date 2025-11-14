using Microsoft.AspNetCore.Identity;
using magazine_app.Models;

namespace magazine_app.Data
{
    public static class DataSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed SuperAdmin User
            await SeedSuperAdminAsync(userManager, configuration);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "SuperAdmin", "Admin", "Uploader", "Customer" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            var superAdminEmail = configuration["SeedAdmin:Email"];
            var superAdminPassword = configuration["SeedAdmin:Password"];

            if (string.IsNullOrEmpty(superAdminEmail) || string.IsNullOrEmpty(superAdminPassword))
            {
                Console.WriteLine("Warning: SuperAdmin credentials not configured in appsettings.json");
                return;
            }

            var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdmin == null)
            {
                var newSuperAdmin = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    FullName = "Super Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
                    Console.WriteLine($"SuperAdmin user created: {superAdminEmail}");
                }
                else
                {
                    Console.WriteLine($"Failed to create SuperAdmin: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure the existing user has SuperAdmin role
                var roles = await userManager.GetRolesAsync(superAdmin);
                if (!roles.Contains("SuperAdmin"))
                {
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                }
            }
        }
    }
}

