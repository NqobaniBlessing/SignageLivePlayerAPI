using System.Security.Claims;

namespace SignageLivePlayerFrontEnd.Services.Interfaces
{
    public interface ISecurityContextService
    {
        ClaimsPrincipal CreateClaimsPrincipal(string token);
    }
}