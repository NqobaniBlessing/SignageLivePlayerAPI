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
            app.MapPost("api/auth/token", [AllowAnonymous] (UserAuthDTO userDTO, 
                ISecurityContextService securityContextService) =>
            {
                var stringToken = securityContextService.CreateJwtToken(userDTO);
                    
                return string.IsNullOrEmpty(stringToken) ? Results.Unauthorized() : 
                Results.Ok(stringToken);
            });
        }
    }
}
