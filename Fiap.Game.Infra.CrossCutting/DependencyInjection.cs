using Fiap.Game.Business.Service;
using Fiap.Game.Domain.Interface;
using Fiap.Game.Domain.Interface.Repository;
using Fiap.Game.Domain.Interface.Service;
using Fiap.Game.Infra.Data;
using Fiap.Game.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.Game.Infra.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGamePlatform(this IServiceCollection services, IConfiguration cfg)
        {
            var connectionString = cfg.GetConnectionString("Default");
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));


            // Data
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<ILibraryRepository, LibraryRepository>();

            // Cross-cutting
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            // Business
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IGameService,GameService>();
            services.AddScoped<ILibraryService,LibraryService>();

            return services;
        }
    }
}
