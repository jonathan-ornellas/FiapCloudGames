using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FiapCloudGames.Payments.Api.Data;
using PaymentsProgram = FiapCloudGames.Payments.Api.Program;

namespace FiapCloudGames.Tests.Integration.Fixtures;

public class PaymentsApiWebApplicationFactory : WebApplicationFactory<PaymentsProgram>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<PaymentsContext>(options =>
            {
                options.UseInMemoryDatabase("PaymentsTestDb");
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
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
