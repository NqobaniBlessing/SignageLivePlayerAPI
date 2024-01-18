namespace SignageLivePlayerAPI.Models.DTOs
{
    public record PlayerDTO(int id, Guid UniqueId, string Name, string SiteAddress1, string SiteAddress2,
        string SiteTown, string SiteCounty, string SitePostCode, string SiteCountry, string CheckInFrequency);
}
