using PetShopSalesAPI.Models;

namespace PetShopSalesAPI.Auth
{
    public interface IAuthUser
    {
        Task<AuthenticationResponse?> Authenticate(AuthenticationRequest model);
    }
}
