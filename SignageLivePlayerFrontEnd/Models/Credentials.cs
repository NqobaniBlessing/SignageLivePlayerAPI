using System.ComponentModel.DataAnnotations;

namespace SignageLivePlayerFrontEnd.Models
{
    public class Credentials
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
