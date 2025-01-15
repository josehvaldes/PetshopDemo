using System.ComponentModel;

namespace PetShopAPI.Models
{
    public class AuthenticationRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }
}
