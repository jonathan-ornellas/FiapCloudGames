namespace FiapCloudGames.Users.Business
{
    public interface IPasswordHasherService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
