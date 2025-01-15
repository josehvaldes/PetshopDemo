using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petshop.Common.Settings;
using PetShop.Service;
using PetShopAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetShopAPI.Auth
{
    public class AuthUser : IAuthUser
    {
        private readonly IUserService _userService;
        private readonly ApiSettings _apiSettings;
        private readonly IPasswdHasher _passwdHasher;
        public AuthUser(IUserService userService, IOptions<ApiSettings> apiSettings, IPasswdHasher passwdHasher) 
        {
            _userService = userService;
            _apiSettings = apiSettings.Value;
            _passwdHasher = passwdHasher;
        }

        public async Task<AuthenticationResponse?> Authenticate(AuthenticationRequest model)
        {
            var userEntity = await _userService.Retrieve(model.Domain, model.Username);

            if (userEntity!=null) 
            {
                var roles = userEntity.roles?.Split(',');
                var user = new User() { Id = userEntity.guid, Roles = roles?? Array.Empty<string>() };

                if (_passwdHasher.VerifyPassword(user, userEntity.hash, model.Password)) 
                {
                    var token = GenerateJwtToken(model.Username, model.Domain, userEntity.roles??"");
                    return new AuthenticationResponse() { Token = token, Username = model.Username };
                }
            }

            return null;
        }

        private string GenerateJwtToken(string username, string domain, string roles)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Domain", domain),
                new Claim("Roles", roles)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiSettings.ApiKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _apiSettings.Issuer,
                audience: _apiSettings.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
