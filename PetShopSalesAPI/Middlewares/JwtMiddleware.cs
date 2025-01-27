using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petshop.Common.Authorization;
using Petshop.Common.Settings;
using PetShop.Service;
using PetShopSalesAPI.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PetShopSalesAPI.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiSettings _apiSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<ApiSettings> apiSettings)
        {
            _next = next;
            _apiSettings = apiSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, userService, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IUserService userService, string token) 
        {
            await Task.Run(() => {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_apiSettings.ApiKey);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clock skew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var username = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                    var domain = jwtToken.Claims.First(x => x.Type == "Domain").Value;
                    var rolesString = jwtToken.Claims.First(x => x.Type == "Roles").Value;

                    
                    if (rolesString != null)
                    {
                        //Attach user to context on successful JWT validation
                        var roles = rolesString.Split(',');
                        if (roles.Contains(Role.User) || roles.Contains(Role.Administrator)) 
                        {
                            var userEntity = userService.Retrieve(domain, username);
                            if (userEntity!=null) 
                            {
                                context.Items["User"] = new User() { UserName = username, Domain = domain, Roles = roles };
                            }
                            //not found in repository then do nothing.
                        }
                        //else: Not authorized. Do nothing.

                    }
                    else 
                    {
                        //Missing role. Not authorized.
                        //Do nothing
                    }
                }
                catch
                {
                    //Do nothing if JWT validation fails
                    // user is not attached to context so the request won't have access to secure routes
                }
            });
          
        }
    }
}
