using System.Security.Claims;

namespace SignageLivePlayerFrontEnd.Models
{
    public class CreatedUser
    {
        public Guid UniqueId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    }
}
