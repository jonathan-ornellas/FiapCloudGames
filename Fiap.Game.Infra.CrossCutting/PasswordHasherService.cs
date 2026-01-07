using Fiap.Game.Domain.Interface.Service;

namespace Fiap.Game.Infra.CrossCutting
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
        public bool IsValid(string pwd) =>
            !string.IsNullOrWhiteSpace(pwd) &&
            System.Text.RegularExpressions.Regex.IsMatch(pwd, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$");

    }
}
