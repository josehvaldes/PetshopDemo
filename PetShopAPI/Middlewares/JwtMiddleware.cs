using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetShop.Application.Auth;
using PetShop.Application.Interfaces;
using PetShop.Application.Settings;
using PetShopAPI.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PetShopAPI.Middlewares
{
    /// <summary>
    /// Middleware to handle JWT authentication and attach user information to the HTTP context.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiSettings _apiSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="apiSettings">The API settings containing the JWT configuration.</param>
        public JwtMiddleware(RequestDelegate next, IOptions<ApiSettings> apiSettings)
        {
            _next = next;
            _apiSettings = apiSettings.Value;
        }

        /// <summary>
        /// Middleware invocation method to process the HTTP request.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <param name="userService">The user service to retrieve user information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, userService, token);

            await _next(context);
        }

        /// <summary>
        /// Attaches the user information to the HTTP context if the JWT token is valid.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <param name="userService">The user service to retrieve user information.</param>
        /// <param name="token">The JWT token from the request header.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task AttachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_apiSettings.ApiKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                var domain = jwtToken.Claims.First(x => x.Type == "Domain").Value;
                var rolesString = jwtToken.Claims.FirstOrDefault(x => x.Type == "Roles")?.Value;

                if (!string.IsNullOrEmpty(rolesString))
                {
                    var roles = rolesString.Split(',');
                    if (roles.Contains(Role.User) || roles.Contains(Role.Administrator))
                    {
                        var userEntity = await userService.Retrieve(domain, username);
                        if (userEntity != null)
                        {
                            context.Items["User"] = new AuthUser
                            {
                                UserName = username,
                                Domain = domain,
                                Roles = roles
                            };
                        }
                    }
                }
            }
            catch
            {
                // Log the error or handle it appropriately
            }
        }
    }
}
