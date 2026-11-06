using TowerCo.Api.Domain;
using TowerCo.Api.Dtos;

namespace TowerCo.Api.Services;

public static class Mappings
{
    public static SiteSummaryDto ToSummary(this Site s) => new(
        s.Id, s.Code, s.Name, s.Country, s.City, s.Status, s.Tower.Type,
        s.Tower.MaxTenants, s.Tower.ActiveTenantCount, s.Tower.AvailableSlots, s.Tower.UtilisationPercent);

    public static SiteDetailDto ToDetail(this Site s) => new(
        s.Id, s.Code, s.Name, s.Country, s.City, s.Latitude, s.Longitude, s.Status, s.CommissionedOn,
        s.Tower.ToDto(),
        s.Tower.Tenancies
            .OrderByDescending(t => t.IsActive).ThenBy(t => t.StartDate)
            .Select(t => t.ToDto())
            .ToList());

    public static TowerDto ToDto(this Tower t) => new(
        t.Id, t.Type, t.HeightMeters, t.PowerSource, t.MaxTenants,
        t.ActiveTenantCount, t.AvailableSlots, t.HasCapacity, t.UtilisationPercent);

    public static TenancyDto ToDto(this Tenancy t) => new(
        t.Id, t.OperatorId, t.Operator.Name, t.StartDate, t.EndDate, t.MonthlyRent, t.IsActive);
}
