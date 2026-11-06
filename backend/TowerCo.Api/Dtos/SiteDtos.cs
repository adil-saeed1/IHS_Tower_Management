using System.ComponentModel.DataAnnotations;
using TowerCo.Api.Domain;

namespace TowerCo.Api.Dtos;

public record SiteSummaryDto(
    int Id, string Code, string Name, string Country, string City, SiteStatus Status,
    TowerType TowerType, int MaxTenants, int ActiveTenants, int AvailableSlots, double UtilisationPercent);

public record SiteDetailDto(
    int Id, string Code, string Name, string Country, string City,
    double Latitude, double Longitude, SiteStatus Status, DateOnly? CommissionedOn,
    TowerDto Tower, IReadOnlyList<TenancyDto> Tenancies);

public record TowerDto(
    int Id, TowerType Type, double HeightMeters, PowerSource PowerSource,
    int MaxTenants, int ActiveTenants, int AvailableSlots, bool HasCapacity, double UtilisationPercent);

// Used for both create and update: a site is always registered together with its tower.
public class SaveSiteRequest
{
    [Required, StringLength(20)] public string Code { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(60)] public string Country { get; set; } = string.Empty;
    [StringLength(60)] public string City { get; set; } = string.Empty;
    [Range(-90, 90)] public double Latitude { get; set; }
    [Range(-180, 180)] public double Longitude { get; set; }
    public SiteStatus Status { get; set; }
    public DateOnly? CommissionedOn { get; set; }

    public TowerType TowerType { get; set; }
    [Range(1, 200)] public double HeightMeters { get; set; }
    [Range(1, 10)] public int MaxTenants { get; set; }
    public PowerSource PowerSource { get; set; }
}
