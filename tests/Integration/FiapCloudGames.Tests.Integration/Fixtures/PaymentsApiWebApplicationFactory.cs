using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FiapCloudGames.Payments.Api.Data;
using FiapCloudGames.EventSourcing;
using PaymentsProgram = FiapCloudGames.Payments.Api.Program;

namespace FiapCloudGames.Tests.Integration.Fixtures;

public class PaymentsApiWebApplicationFactory : WebApplicationFactory<PaymentsProgram>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o PaymentsContext existente (SQL Server)
            var paymentsContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
            
            if (paymentsContextDescriptor != null)
            {
                services.Remove(paymentsContextDescriptor);
            }

            var paymentsContextImplementation = services.SingleOrDefault(
                d => d.ServiceType == typeof(PaymentsContext));
            
            if (paymentsContextImplementation != null)
            {
                services.Remove(paymentsContextImplementation);
            }

            // Remove o EventSourcingContext existente (SQL Server)
            var eventSourcingContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EventSourcingContext>));
            
            if (eventSourcingContextDescriptor != null)
            {
                services.Remove(eventSourcingContextDescriptor);
            }

            var eventSourcingContextImplementation = services.SingleOrDefault(
                d => d.ServiceType == typeof(EventSourcingContext));
            
            if (eventSourcingContextImplementation != null)
            {
                services.Remove(eventSourcingContextImplementation);
            }

            // Adiciona os DbContexts com InMemory
            services.AddDbContext<PaymentsContext>(options =>
            {
                options.UseInMemoryDatabase($"PaymentsTestDb_{Guid.NewGuid()}");
            });

            services.AddDbContext<EventSourcingContext>(options =>
            {
                options.UseInMemoryDatabase($"EventSourcingTestDb_{Guid.NewGuid()}");
            });

            // Garante que os bancos estão criados
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            
            var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            paymentsDbContext.Database.EnsureCreated();

            var eventSourcingDbContext = scope.ServiceProvider.GetRequiredService<EventSourcingContext>();
            eventSourcingDbContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Test");
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        return client;
    }
}
