using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PipelineCacher.Client
{
    internal class UserInfoClaims : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.HasClaim(c => c.Type == ClaimTypes.Country))
            {
                ClaimsIdentity id = (ClaimsIdentity)principal.Identity;                
                id.AddClaim(new Claim(ClaimTypes.Country, "Canada"));
                //principal.AddIdentity(id);
                return Task.FromResult(new ClaimsPrincipal(id));
            }

            return Task.FromResult(principal);
        }
    }
}