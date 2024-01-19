namespace SignageLivePlayerAPI.Models.DTOs
{
    public record UserDTO(Guid UniqueId, string UserName, Dictionary<string, string> Claims);
}
