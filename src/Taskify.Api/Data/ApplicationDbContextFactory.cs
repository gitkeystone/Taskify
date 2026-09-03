using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Taskify.Api.Data;

/// <summary>
/// Design-time factory so <c>dotnet ef</c> can create the context to generate migrations
/// without running the full application (which would execute startup seeding).
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=taskify")
            .Options;

        return new ApplicationDbContext(options);
    }
}
