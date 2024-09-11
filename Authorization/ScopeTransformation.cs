using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace GameStore.Authorization;

public class ScopeTransformation : IClaimsTransformation
{
    private const string scopeClaimName = "scope";
 
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var scopeClaim = principal.FindFirst(scopeClaimName);
        if (scopeClaim is null)
        {
            return Task.FromResult(principal);
        }
        
        var scopes = scopeClaim.Value.Split(" ");
        
        var originalIdentity = principal.Identity as ClaimsIdentity;
        var identity = new ClaimsIdentity(originalIdentity);
        
        var originalScopeClaim = identity.Claims.FirstOrDefault(c => c.Type == scopeClaimName);
        if (originalScopeClaim is null)
        {
            identity.RemoveClaim(originalScopeClaim);
        }
        identity.AddClaims(scopes.Select(scope => new Claim(scopeClaimName, scope)));
        
        return Task.FromResult(new ClaimsPrincipal(identity));
    }
}