using SignageLivePlayerAPI.Models.DTOs;

namespace SignageLivePlayerAPI.Services.Interfaces
{
    public interface ISecurityContextService
    {
        string CreateJwtToken(UserAuthDTO userDTO);
    }
}