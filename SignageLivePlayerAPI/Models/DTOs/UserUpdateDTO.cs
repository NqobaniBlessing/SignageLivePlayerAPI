namespace SignageLivePlayerAPI.Models.DTOs
{
    public record UserUpdateDTO(Guid UniqueId, string UserName, string Password, Dictionary<string, string> Claims);
}
