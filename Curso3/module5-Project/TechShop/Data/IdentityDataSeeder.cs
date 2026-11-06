using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechShop.Models;

namespace DataManagement.Data;

public static class IdentityDataSeeder
{
    public const string CustomerRole = "Customer";
    public const string AdminRole = "Admin";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync(cancellationToken);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IdentityDataSeeder));

        foreach (var roleName in new[] { CustomerRole, AdminRole })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var createRole = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!createRole.Succeeded)
                {
                    logger.LogError(
                        "Failed to create role {Role}: {Errors}",
                        roleName,
                        string.Join("; ", createRole.Errors.Select(e => e.Description)));
                }
            }
        }

        var adminEmail = configuration["Seed:AdminEmail"];
        var adminPassword = configuration["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("Seed admin skipped: Seed:AdminEmail or Seed:AdminPassword is not set.");
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator"
            };

            var createUser = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createUser.Succeeded)
            {
                logger.LogError(
                    "Failed to create seed admin user: {Errors}",
                    string.Join("; ", createUser.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
        {
            var addRole = await userManager.AddToRoleAsync(adminUser, AdminRole);
            if (!addRole.Succeeded)
            {
                logger.LogError(
                    "Failed to assign Admin role to seed user: {Errors}",
                    string.Join("; ", addRole.Errors.Select(e => e.Description)));
            }
        }
    }
}
