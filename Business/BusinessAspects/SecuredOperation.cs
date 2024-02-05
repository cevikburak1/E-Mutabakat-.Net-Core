using Castle.DynamicProxy;
using Core.Extensions;
using Core.Utilities.Interception;
using Core.Utilities.IOC;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.BusinessAspects;

public class SecuredOperation :MethodInterception
{
    private string[] _roles;
    private IHttpContextAccessor _httpContextAccessor;

    public SecuredOperation()
    {
        _httpContextAccessor = ServiceTool.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    }
    public SecuredOperation(string roles)
    {
        _roles = roles.Split(",");
        _httpContextAccessor = ServiceTool.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    }

    protected override void OnBefore(IInvocation invocation)
    {
        if (_roles != null)
        {
            // hata audience de harf hatası yaptığım içinmiş :) yarım saat cebelleştim
            
            var roleClaims = _httpContextAccessor.HttpContext.User.ClaimsRoles();
            foreach (var role in _roles)
            {
                if (roleClaims.Contains(role))
                {
                    return;
                }
            }

            throw new Exception("İşlem için yetkiniz bulunmuyor");

        }
        else//bir çağırdığın metodu açar mısın
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            if (claims.Count() > 0)
            {
                return;
            }
            throw new Exception("İşlem için yetkiniz bulunmuyor");
        }

    }
}
