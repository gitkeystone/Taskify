namespace Taskify.Api.Models;

/// <summary>The role a predefined user holds.</summary>
public enum UserRole
{
    ProductManager = 1,
    Engineer = 2
}

/// <summary>A predefined team member. Exactly five exist in phase 1.</summary>
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
