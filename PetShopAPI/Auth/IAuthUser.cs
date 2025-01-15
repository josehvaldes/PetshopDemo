using PetShopAPI.Models;

namespace PetShopAPI.Auth
{
    public interface IAuthUser
    {
        Task<AuthenticationResponse?> Authenticate(AuthenticationRequest model);
    }
}
