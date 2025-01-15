using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PetShopAPI.Auth
{
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string[] _roles;
        public AuthorizeAttribute(params string[] roles) : base()
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User?)context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new JsonResult(new { Error = new[] { "Unauthorized" } }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                var allowedRoles = _roles;
                if (allowedRoles.Length > 0) 
                {
                    var userroles = user.Roles;
                    
                    var intersect = allowedRoles.Intersect(userroles).ToList();

                    if (!intersect.Any()) 
                    {
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }                
            }
        }
    }
}
