using Microsoft.EntityFrameworkCore;
using Taskify.Api.Models;

namespace Taskify.Api.Data;

/// <summary>Seeds the five predefined users and three sample projects.</summary>
public static class SeedData
{
    public static readonly Guid ProductManagerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Engineer1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Engineer2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid Engineer3Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid Engineer4Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;

        db.Users.AddRange(
            new User { Id = ProductManagerId, Name = "Alex Morgan", Role = UserRole.ProductManager, CreatedAt = now },
            new User { Id = Engineer1Id, Name = "Sam Lee", Role = UserRole.Engineer, CreatedAt = now },
            new User { Id = Engineer2Id, Name = "Priya Patel", Role = UserRole.Engineer, CreatedAt = now },
            new User { Id = Engineer3Id, Name = "Jordan Kim", Role = UserRole.Engineer, CreatedAt = now },
            new User { Id = Engineer4Id, Name = "Taylor Chen", Role = UserRole.Engineer, CreatedAt = now });

        db.Projects.AddRange(
            new Project { Id = Guid.NewGuid(), Name = "Website Redesign", CreatedById = ProductManagerId, CreatedAt = now },
            new Project { Id = Guid.NewGuid(), Name = "Mobile App Launch", CreatedById = ProductManagerId, CreatedAt = now },
            new Project { Id = Guid.NewGuid(), Name = "Internal Tools", CreatedById = ProductManagerId, CreatedAt = now });

        await db.SaveChangesAsync();
    }
}
