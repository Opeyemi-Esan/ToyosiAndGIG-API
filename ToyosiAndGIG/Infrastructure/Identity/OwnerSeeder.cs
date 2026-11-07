using Microsoft.AspNetCore.Identity;
using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Identity
{
    public static class OwnerSeeder
    {
        public static async Task SeedOwnerAsync(
            UserManager<User> userManager,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILogger logger)
        {
            var existingOwners = await userManager.GetUsersInRoleAsync(Roles.Owner);
            if (existingOwners.Any())
            {
                return;
            }

            var email = configuration["InitialOwner:Email"];
            var password = configuration["InitialOwner:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                if (environment.IsProduction())
                {
                    logger.LogCritical(
                        "!!! NO OWNER ACCOUNT EXISTS AND InitialOwner:Email/Password ARE NOT CONFIGURED !!! " +
                        "This is a PRODUCTION environment — the platform has no Owner and nobody can create " +
                        "Managers or employees until this is fixed. Set InitialOwner:Email and " +
                        "InitialOwner:Password via environment variables and restart the app.");
                }
                else
                {
                    logger.LogWarning(
                        "No Owner account exists and InitialOwner:Email/Password are not configured. " +
                        "Set them via user-secrets to seed the first Owner (not required for {Environment}).",
                        environment.EnvironmentName);
                }

                return;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = configuration["InitialOwner:FirstName"] ?? "System",
                LastName = configuration["InitialOwner:LastName"] ?? "Owner",
                Email = email,
                UserName = email,
                PhoneNumber = configuration["InitialOwner:PhoneNumber"] ?? "00000000000",
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to seed initial Owner account: {Errors}", errors);
                return;
            }

            await userManager.AddToRoleAsync(user, Roles.Owner);
            logger.LogInformation("Initial Owner account seeded for {Email}", email);
        }
    }
}
