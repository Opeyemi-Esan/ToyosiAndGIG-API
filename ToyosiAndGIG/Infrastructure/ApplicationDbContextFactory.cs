namespace ToyosiAndGIG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

// Used ONLY by EF Core CLI/PMC tools (Add-Migration, Update-Database) at design time.
// Never invoked at runtime by the actual app — Program.cs still owns the real
// registration via AddInfrastructure(). This exists purely so migration commands
// don't need to fully boot the app (JWT config, Serilog, auth, etc.) just to find
// the DbContext.
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Reads appsettings.json from the API project so the connection string
        // matches what the app actually uses at runtime.
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.Exists(basePath) ? basePath : Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<ApplicationDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection not found. Check appsettings.json in the API project.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
