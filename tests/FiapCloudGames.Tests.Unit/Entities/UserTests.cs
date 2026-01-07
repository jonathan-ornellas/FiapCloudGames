using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ValidData_CreatesUser()
    {
        var name = "João Silva";
        var email = new Email("joao@example.com");
        var password = "senha123";
        var role = "User";

        var user = new User(name, email, password, role);

        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Equal(role, user.Role);
    }

    [Fact]
    public void Constructor_WithoutRole_DefaultsToUser()
    {
        var user = new User("João Silva", new Email("joao@example.com"), "senha123");

        Assert.Equal("User", user.Role);
    }

    [Fact]
    public void Constructor_WithAdminRole_SetsCorrectly()
    {
        var user = new User("Admin User", new Email("admin@example.com"), "senha123", "Admin");

        Assert.Equal("Admin", user.Role);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    public void Constructor_DifferentRoles_CreatesCorrectly(string role)
    {
        var user = new User("Test User", new Email("test@example.com"), "senha123", role);

        Assert.Equal(role, user.Role);
    }

    [Fact]
    public void User_CanUpdateRole()
    {
        var user = new User("Test User", new Email("test@example.com"), "senha123", "User");

        user.Role = "Admin";

        Assert.Equal("Admin", user.Role);
    }

    [Fact]
    public void User_CanUpdateName()
    {
        var user = new User("Old Name", new Email("test@example.com"), "senha123");

        user.Name = "New Name";

        Assert.Equal("New Name", user.Name);
    }

    [Fact]
    public void User_EmailIsValueObject()
    {
        var email = new Email("test@example.com");
        var user = new User("Test", email, "senha123");

        Assert.Equal("test@example.com", user.Email.Value);
    }
}
