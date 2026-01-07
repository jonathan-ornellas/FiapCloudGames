using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FiapCloudGames.Users.Api.Data;
using UsersProgram = FiapCloudGames.Users.Api.Program;

namespace FiapCloudGames.Tests.Integration.Fixtures;

public class UsersApiWebApplicationFactory : WebApplicationFactory<UsersProgram>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext existente (SQL Server)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UsersContext>));
            
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove também a implementação concreta
            var dbContextImplementation = services.SingleOrDefault(
                d => d.ServiceType == typeof(UsersContext));
            
            if (dbContextImplementation != null)
            {
                services.Remove(dbContextImplementation);
            }

            // Adiciona o DbContext com InMemory
            services.AddDbContext<UsersContext>(options =>
            {
                options.UseInMemoryDatabase($"UsersTestDb_{Guid.NewGuid()}");
            });

            // Garante que o banco está criado
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();
            dbContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Test");
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        return client;
    }
}
