namespace SignageLivePlayerAPI.Models
{
    public class User
    {
        public Guid UniqueId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
        public byte[] Salt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
