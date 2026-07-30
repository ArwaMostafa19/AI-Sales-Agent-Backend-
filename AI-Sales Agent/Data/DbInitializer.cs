using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Seed Roles
            var roles = new[] { Roles.Admin, Roles.Seller };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // Seed Default Admin User
            var adminEmail = "admin@ai-commerce.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@12345");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }

            // Assign Seller role to existing users who have no roles (to fix old accounts)
            foreach (var user in userManager.Users.ToList())
            {
                var userRoles = await userManager.GetRolesAsync(user);
                if (!userRoles.Any())
                {
                    await userManager.AddToRoleAsync(user, Roles.Seller);
                }
            }
        }
    }
}
