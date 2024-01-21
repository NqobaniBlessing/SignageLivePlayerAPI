using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLiveAuthEndpoints
    {
        public static void MapSignageLiveAuthEndpoints(this WebApplication app)
        {
            app.MapPost("api/auth/token", [AllowAnonymous] (IUserService userService, UserAuthDTO userDTO,
                IConfiguration configuration, IMapper mapper) =>
            {
                var claims = new List<Claim>();

                var users = userService.GetAllUsers();
                var userFromDataStore = users?.Find(u => u.UserName.Equals(userDTO.UserName));

                if (userFromDataStore == null)
                    return Results.NotFound();

                bool isAuthenticated = userService.AuthenticateUser(userDTO);

                if (isAuthenticated)
                {
                    var issuer = configuration["Jwt:Issuer"];
                    var audience = configuration["Jwt:Audience"];
                    var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? throw new ArgumentNullException());

                    // Add custom claims specific to the authenticated user
                    foreach (var kvp in userFromDataStore.Claims)
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value));
                    }

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
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);
                    var stringToken = tokenHandler.WriteToken(token);

                    return Results.Ok(stringToken);
                }

                return Results.Unauthorized();
            });
        }
    }
}
