using PetShopSalesAPI.Models;

namespace PetShopSalesAPI.Auth
{
    public interface IUserAuthentication
    {
        Task<AuthenticationResponse?> Authenticate(AuthenticationRequest model);
    }
}
