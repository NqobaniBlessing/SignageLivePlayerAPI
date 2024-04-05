using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SignageLivePlayerFrontEnd.Services.Interfaces;

namespace SignageLivePlayerFrontEnd.Services
{
    public class SecurityContextService : ISecurityContextService
    {
        public ClaimsPrincipal CreateClaimsPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var trimmedToken = token.Replace("\"", "").Trim();

            if (!string.IsNullOrEmpty(token))
            {
                // Deserialize token and create an authentication cookie
                var jwtToken = tokenHandler.ReadJwtToken(trimmedToken);

                var claims = jwtToken.Claims.ToList();
                var name = claims.FirstOrDefault(c => c.Type.Equals("unique_name"))?.Value;

                claims.Add(new (ClaimTypes.Name, name ?? string.Empty));

                var identity = new ClaimsIdentity(claims, "Signage_Live");

                var principal = new ClaimsPrincipal(identity);

                return principal;
            }

            return new ClaimsPrincipal();
        }
    }
}
