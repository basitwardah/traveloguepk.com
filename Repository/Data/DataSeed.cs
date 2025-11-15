using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace Repository.Data
{
    public static class DataSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed SuperAdmin User
            await SeedSuperAdminAsync(userManager, configuration);

            // Seed Categories
            await SeedCategoriesAsync(context);
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

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            // Check if categories already exist
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            var categories = new[]
            {
                new Category
                {
                    Name = "Adventure",
                    Slug = "adventure",
                    Description = "Thrilling adventures and exciting journeys",
                    IconClass = "fa-mountain",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Category
                {
                    Name = "Explore",
                    Slug = "explore",
                    Description = "Discover new places and cultures",
                    IconClass = "fa-compass",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Category
                {
                    Name = "Discover",
                    Slug = "discover",
                    Description = "Uncover hidden gems and treasures",
                    IconClass = "fa-map-marked-alt",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new Category
                {
                    Name = "Journey",
                    Slug = "journey",
                    Description = "Epic journeys and travel stories",
                    IconClass = "fa-route",
                    DisplayOrder = 4,
                    IsActive = true
                },
                new Category
                {
                    Name = "Culture",
                    Slug = "culture",
                    Description = "Cultural experiences and heritage",
                    IconClass = "fa-landmark",
                    DisplayOrder = 5,
                    IsActive = true
                },
                new Category
                {
                    Name = "Nature",
                    Slug = "nature",
                    Description = "Natural wonders and wildlife",
                    IconClass = "fa-tree",
                    DisplayOrder = 6,
                    IsActive = true
                },
                new Category
                {
                    Name = "History",
                    Slug = "history",
                    Description = "Historical sites and ancient civilizations",
                    IconClass = "fa-book",
                    DisplayOrder = 7,
                    IsActive = true
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            Console.WriteLine($"Seeded {categories.Length} default categories");
        }
    }
}

