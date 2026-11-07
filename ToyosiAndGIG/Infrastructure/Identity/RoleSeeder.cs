using Microsoft.AspNetCore.Identity;
using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Identity
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { Roles.Customer, Roles.Member, Roles.Manager, Roles.Owner };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
