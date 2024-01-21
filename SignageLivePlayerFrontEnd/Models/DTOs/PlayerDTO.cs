using System.ComponentModel.DataAnnotations;

namespace SignageLivePlayerFrontEnd.Models.DTOs
{
    public class PlayerDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SiteAddress1 { get; set; } = string.Empty;
        public string SiteAddress2 { get; set; } = string.Empty;

        [Required]
        public string SiteTown { get; set; } = string.Empty;
        public string SiteCounty { get; set; } = string.Empty;
        public string SitePostCode { get; set; } = string.Empty;

        [Required]
        public string SiteCountry { get; set; } = string.Empty;

        [Required]
        public string CheckInFrequency { get; set; } = string.Empty;
    }
}
