using PetShopAPI.Models;

namespace PetShopAPI.Auth
{
    public interface IUserAuthentication
    {
        Task<AuthenticationResponse?> Authenticate(AuthenticationRequest model);
    }
}
