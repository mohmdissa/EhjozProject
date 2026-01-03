using EhjozProject.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EhjozProject.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations if needed
            await context.Database.MigrateAsync();

            // Check if admin user already exists
            var adminEmail = "admin@ehjoz.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    Role = "Admin",
                    IsApproved = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure existing user has Admin role
                if (adminUser.Role != "Admin")
                {
                    adminUser.Role = "Admin";
                    adminUser.IsApproved = true;
                    await userManager.UpdateAsync(adminUser);
                }
                
                // Reset password to ensure it's correct
                var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                var resetResult = await userManager.ResetPasswordAsync(adminUser, token, "Admin123!");
                if (!resetResult.Succeeded)
                {
                    // If reset fails, remove password and set it again
                    await userManager.RemovePasswordAsync(adminUser);
                    await userManager.AddPasswordAsync(adminUser, "Admin123!");
                }
            }
        }
    }
}

