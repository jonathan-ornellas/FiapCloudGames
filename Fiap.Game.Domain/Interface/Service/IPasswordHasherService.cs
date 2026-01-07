namespace Fiap.Game.Domain.Interface.Service
{
    public interface IPasswordHasherService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
        bool IsValid(string pwd);
    }
}
