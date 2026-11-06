using Microsoft.EntityFrameworkCore;
using TowerCo.Api.Data;
using TowerCo.Api.Domain;
using TowerCo.Api.Dtos;

namespace TowerCo.Api.Services;

public interface ISiteService
{
    Task<IReadOnlyList<SiteSummaryDto>> SearchAsync(string? search, string? country, SiteStatus? status);
    Task<SiteDetailDto> GetAsync(int id);
    Task<IReadOnlyList<string>> GetCountriesAsync();
    Task<SiteDetailDto> CreateAsync(SaveSiteRequest request);
    Task<SiteDetailDto> UpdateAsync(int id, SaveSiteRequest request);
}

public class SiteService(AppDbContext db) : ISiteService
{
    public async Task<IReadOnlyList<SiteSummaryDto>> SearchAsync(string? search, string? country, SiteStatus? status)
    {
        var query = db.Sites
            .Include(s => s.Tower).ThenInclude(t => t.Tenancies)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s => s.Code.ToLower().Contains(term)
                                  || s.Name.ToLower().Contains(term)
                                  || s.City.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(s => s.Country == country);

        if (status is not null)
            query = query.Where(s => s.Status == status);

        var sites = await query.OrderBy(s => s.Code).ToListAsync();
        return sites.Select(s => s.ToSummary()).ToList();
    }

    public async Task<SiteDetailDto> GetAsync(int id) => (await LoadAsync(id)).ToDetail();

    public async Task<IReadOnlyList<string>> GetCountriesAsync() =>
        await db.Sites.Select(s => s.Country).Distinct().OrderBy(c => c).ToListAsync();

    public async Task<SiteDetailDto> CreateAsync(SaveSiteRequest request)
    {
        await EnsureCodeIsUniqueAsync(request.Code, excludeId: null);

        var site = new Site { Tower = new Tower() };
        Apply(site, request);
        db.Sites.Add(site);
        await db.SaveChangesAsync();

        return site.ToDetail();
    }

    public async Task<SiteDetailDto> UpdateAsync(int id, SaveSiteRequest request)
    {
        var site = await LoadAsync(id, tracked: true);
        await EnsureCodeIsUniqueAsync(request.Code, excludeId: id);

        if (request.MaxTenants < site.Tower.ActiveTenantCount)
            throw new BusinessRuleException(
                $"Capacity cannot be lower than the current {site.Tower.ActiveTenantCount} active tenants.");

        Apply(site, request);
        await db.SaveChangesAsync();

        return site.ToDetail();
    }

    private async Task<Site> LoadAsync(int id, bool tracked = false)
    {
        var query = db.Sites
            .Include(s => s.Tower).ThenInclude(t => t.Tenancies).ThenInclude(t => t.Operator)
            .AsQueryable();

        if (!tracked) query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(s => s.Id == id)
               ?? throw new NotFoundException($"Site {id} was not found.");
    }

    private async Task EnsureCodeIsUniqueAsync(string code, int? excludeId)
    {
        var normalised = code.Trim().ToUpper();
        if (await db.Sites.AnyAsync(s => s.Code == normalised && s.Id != excludeId))
            throw new BusinessRuleException($"Site code '{normalised}' is already in use.");
    }

    private static void Apply(Site site, SaveSiteRequest r)
    {
        site.Code = r.Code.Trim().ToUpper();
        site.Name = r.Name.Trim();
        site.Country = r.Country.Trim();
        site.City = r.City.Trim();
        site.Latitude = r.Latitude;
        site.Longitude = r.Longitude;
        site.Status = r.Status;
        site.CommissionedOn = r.CommissionedOn;

        site.Tower.Type = r.TowerType;
        site.Tower.HeightMeters = r.HeightMeters;
        site.Tower.MaxTenants = r.MaxTenants;
        site.Tower.PowerSource = r.PowerSource;
    }
}
