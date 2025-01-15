using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShopAPI.Models
{
    public class AuthenticationResponse
    {
        public required string Username { get; set; }
        public required string Token { get; set; }

    }
}
