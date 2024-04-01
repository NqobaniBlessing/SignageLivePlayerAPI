using SignageLivePlayerAPI.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace SignageLivePlayerAPI.Services.Interfaces
{
    public interface ISecurityContextService
    {
        string CreateJwtToken(UserAuthDTO userDTO);
    }
}