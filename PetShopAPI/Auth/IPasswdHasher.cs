namespace PetShopAPI.Auth
{
    public interface IPasswdHasher
    {
        string HashPassword(AuthUser user, string password);
        bool VerifyPassword(AuthUser user, string hashedPassword, string password);
    }
}
