using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Enum;
using Fiap.Game.Domain.ValueObjects;
using Fiap.Game.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.CrossCutting
{
    public static class SeedData
    {
        public static async Task EnsureAdminAsync(AppDbContext db)
        {
            var adminEmail = new Email("admin@fcg.local");

            if (!await db.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");

                var adminUser = new User(
                    "FCG Admin",
                    adminEmail,
                    hashedPassword,
                    Role.Admin
                );

                db.Users.Add(adminUser);
                await db.SaveChangesAsync();
            }
        }
    }
}
