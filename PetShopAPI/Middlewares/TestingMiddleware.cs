using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetShop.Application.Interfaces;
using PetShop.Application.Settings;
using PetShopAPI.Auth;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PetShopAPI.Middlewares
{
    public class TestingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiSettings _apiSettings;

        public static bool TestingMode = false;

        public TestingMiddleware(RequestDelegate next, IOptions<ApiSettings> apiSettings)
        {
            _next = next;
            _apiSettings = apiSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            context.Items["User"] = new AuthUser() { UserName = "Test", Domain = "bo", Roles = new string[] { "administrator" } };
        }

    }
}
