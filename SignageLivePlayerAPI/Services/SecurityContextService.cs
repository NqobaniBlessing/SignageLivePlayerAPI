using Microsoft.IdentityModel.Tokens;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignageLivePlayerAPI.Services
{
    public class SecurityContextService : ISecurityContextService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public SecurityContextService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public string CreateJwtToken(UserAuthDTO userDTO)
        {
            var claims = new List<Claim>();

            var (isAuthenticated, userFromDataStore) = _userService.AuthenticateUser(userDTO);

            if (isAuthenticated && userFromDataStore != null)
            {
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ??
                    throw new NullReferenceException());

                // Add custom claims specific to the authenticated user
                foreach (var kvp in userFromDataStore.Claims)
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value));
                }

                // Extract a user friendly user name from the company email address
                var userName = claims.FirstOrDefault(c => c.Type.Equals("email"))?.Value;
                userName = userName.Split(new[] { '@' })[0];
                userName = char.ToUpper(userName[0]) + userName[1..];

                claims.Add(new Claim(ClaimTypes.Name, userName));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
                var stringToken = tokenHandler.WriteToken(token);

                return stringToken;
            }

            return string.Empty;
        }
    }
}
