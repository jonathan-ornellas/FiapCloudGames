using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FiapCloudGames.Games.Api.Data;
using GamesProgram = FiapCloudGames.Games.Api.Program;

namespace FiapCloudGames.Tests.Integration.Fixtures;

public class GamesApiWebApplicationFactory : WebApplicationFactory<GamesProgram>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GamesContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<GamesContext>(options =>
            {
                options.UseInMemoryDatabase("GamesTestDb");
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GamesContext>();
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
