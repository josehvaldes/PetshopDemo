using Microsoft.AspNetCore.Identity;

namespace PetShopSalesAPI.Auth
{
    public class User : IdentityUser
    {
        public string Domain { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
