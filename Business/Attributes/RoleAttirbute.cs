using Microsoft.AspNetCore.Mvc.Filters;

namespace Business.Attributes;

public class RoleAttirbute : Attribute, IAuthorizationFilter
{
    private readonly string _role;
    public RoleAttirbute(string role)
    {
        _role = role;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var _context = context.HttpContext.Request.Headers;
    }
}
