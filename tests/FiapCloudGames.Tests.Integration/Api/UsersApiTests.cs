using System.Net;
using System.Net.Http.Json;
using FiapCloudGames.Users.Api.DTOs;
using Xunit;

namespace FiapCloudGames.Tests.Integration.Api;

public class UsersApiTests
{
    [Fact]
    public void RegisterUserDto_ValidDto_HasCorrectProperties()
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Test@123"
        };

        Assert.Equal("Test User", dto.Name);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("Test@123", dto.Password);
    }

    [Fact]
    public void RegisterUserDto_EmptyValues_AllowsCreation()
    {
        var dto = new RegisterUserDto
        {
            Name = "",
            Email = "",
            Password = ""
        };

        Assert.Empty(dto.Name);
        Assert.Empty(dto.Email);
        Assert.Empty(dto.Password);
    }
}
