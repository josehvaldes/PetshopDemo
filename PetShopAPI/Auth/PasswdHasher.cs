using Microsoft.AspNetCore.Identity;

namespace PetShopAPI.Auth
{
    public class PasswdHasher : IPasswdHasher
    {
        private readonly IPasswordHasher<AuthUser> _passwordHasher;

        public PasswdHasher(IPasswordHasher<AuthUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(AuthUser user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(AuthUser user, string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
            // if required, you can handle if result is SuccessRehashNeeded
            return result == PasswordVerificationResult.Success;
        }
    }
}
