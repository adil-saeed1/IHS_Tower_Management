using Microsoft.EntityFrameworkCore;
using TowerCo.Api.Data;
using TowerCo.Api.Domain;
using TowerCo.Api.Dtos;

namespace TowerCo.Api.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync();
}

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardDto> GetAsync()
    {
        // Portfolio is small, so aggregating in memory keeps this simple and readable.
        var sites = await db.Sites
            .Include(s => s.Tower).ThenInclude(t => t.Tenancies)
            .AsNoTracking()
            .ToListAsync();

        // Utilisation only counts towers that can actually be leased.
        var leasable = sites.Where(s => s.Status == SiteStatus.Active).ToList();
        var capacity = leasable.Sum(s => s.Tower.MaxTenants);
        var tenancies = leasable.Sum(s => s.Tower.ActiveTenantCount);

        var byCountry = sites
            .GroupBy(s => s.Country)
            .Select(g =>
            {
                var active = g.Where(s => s.Status == SiteStatus.Active).ToList();
                var cap = active.Sum(s => s.Tower.MaxTenants);
                var ten = active.Sum(s => s.Tower.ActiveTenantCount);
                return new CountryStatDto(g.Key, g.Count(), ten, cap, Percent(ten, cap));
            })
            .OrderBy(c => c.Country)
            .ToList();

        var byStatus = Enum.GetValues<SiteStatus>()
            .Select(status => new StatusStatDto(status, sites.Count(s => s.Status == status)))
            .ToList();

        var mapPoints = sites
            .Select(s => new MapPointDto(s.Id, s.Code, s.Name, s.Latitude, s.Longitude, s.Status,
                s.Tower.ActiveTenantCount, s.Tower.MaxTenants, s.Tower.UtilisationPercent))
            .ToList();

        var revenue = sites.SelectMany(s => s.Tower.Tenancies).Where(t => t.IsActive).Sum(t => t.MonthlyRent);

        return new DashboardDto(sites.Count, leasable.Count, tenancies, capacity, Percent(tenancies, capacity),
            revenue, byCountry, byStatus, mapPoints);
    }

    private static double Percent(int part, int total) => total == 0 ? 0 : Math.Round(100.0 * part / total, 1);
}
