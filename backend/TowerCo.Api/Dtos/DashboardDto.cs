using TowerCo.Api.Domain;

namespace TowerCo.Api.Dtos;

public record DashboardDto(
    int TotalSites,
    int ActiveSites,
    int TotalTenancies,
    int TotalCapacity,
    double UtilisationPercent,
    decimal MonthlyRevenue,
    IReadOnlyList<CountryStatDto> ByCountry,
    IReadOnlyList<StatusStatDto> ByStatus,
    IReadOnlyList<MapPointDto> MapPoints);

public record CountryStatDto(string Country, int Sites, int Tenancies, int Capacity, double UtilisationPercent);

public record StatusStatDto(SiteStatus Status, int Sites);

public record MapPointDto(
    int Id, string Code, string Name, double Latitude, double Longitude,
    SiteStatus Status, int ActiveTenants, int MaxTenants, double UtilisationPercent);
