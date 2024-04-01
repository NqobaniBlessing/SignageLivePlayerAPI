using Microsoft.AspNetCore.Authorization;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLiveAuthEndpoints
    {
        public static void MapSignageLiveAuthEndpoints(this WebApplication app)
        {
            app.MapPost("api/auth/token", [AllowAnonymous] (UserAuthDTO userDTO, 
                ISecurityContextService securityContextService) =>
            {
                if (userDTO == null) return Results.BadRequest();

                var token = securityContextService.CreateJwtToken(userDTO);
                    
                return string.IsNullOrEmpty(token) ? Results.Unauthorized() : 
                Results.Ok(token);
            });
        }
    }
}
