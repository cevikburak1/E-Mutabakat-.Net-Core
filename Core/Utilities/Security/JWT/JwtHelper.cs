using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Security.JWT
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private TokenOptions _tokenOptions;
        DateTime _accessTokenExpiration;
        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }
        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims, int companyid)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signinCredentals = SignInCredentialsHelper.CreateSignInCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signinCredentals, operationClaims, companyid);
            var jwtSecurityTokenHndler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHndler.WriteToken(jwt);
            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration,
                CompanyId = companyid

            };
        }

        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions,User user,SigningCredentials signingCredentials, List<OperationClaim> operationClaims,int companyid)
        {
            var jwt = new JwtSecurityToken(issuer: tokenOptions.Issuer, audience: tokenOptions.Audience, expires: _accessTokenExpiration, notBefore: DateTime.Now, claims: SetClaims(user, operationClaims, companyid), signingCredentials: signingCredentials);
            return jwt;
        }
        private IEnumerable<Claim> SetClaims(User user,List<OperationClaim> operationClaims, int companyid)
        {
            var claims =  new List<Claim>();
            claims.AddNameIdentitfier(user.Id.ToString());
            claims.AddEmail(user.Email);
            claims.AddName($"{user.Name}");
            claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
            claims.AddCompany(companyid.ToString());
            return claims;
        }
    }
}
