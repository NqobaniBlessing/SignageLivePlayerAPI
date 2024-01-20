using System.Security.Claims;

namespace SignageLivePlayerFrontEnd.Models
{
    public class CreatedUser
    {
        public Guid UniqueId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}
