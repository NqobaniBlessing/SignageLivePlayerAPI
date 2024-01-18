using System.ComponentModel.DataAnnotations;

namespace SignageLivePlayerAPI.Models
{
    public class Player
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SiteAddress1 { get; set; } = string.Empty;
        
        public string SiteAddress2 { get; set;} = string.Empty;

        [Required]
        public string SiteTown { get; set; } = string.Empty;

        public string SiteCounty { get; set; } = string.Empty;
        
        public string SitePostCode { get; set; } = string.Empty;

        [Required]
        public string SiteCountry { get; set; } = string.Empty;

        [Required]
        public string CheckInFrequency { get; set; } = string.Empty;
        
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
