namespace PetShopAPI.Auth
{
    public interface IPasswdHasher
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string hashedPassword, string password);
    }
}
