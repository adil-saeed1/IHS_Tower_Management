using Microsoft.EntityFrameworkCore;
using TowerCo.Api.Data;
using TowerCo.Api.Domain;
using TowerCo.Api.Dtos;

namespace TowerCo.Api.Services;

public interface ITenancyService
{
    Task<IReadOnlyList<OperatorDto>> GetOperatorsAsync();
    Task<TenancyDto> AddAsync(int siteId, AddTenancyRequest request);
    Task EndAsync(int tenancyId);
}

public class TenancyService(AppDbContext db) : ITenancyService
{
    public async Task<IReadOnlyList<OperatorDto>> GetOperatorsAsync() =>
        await db.Operators.OrderBy(o => o.Name).Select(o => new OperatorDto(o.Id, o.Name)).ToListAsync();

    public async Task<TenancyDto> AddAsync(int siteId, AddTenancyRequest request)
    {
        var site = await db.Sites
                       .Include(s => s.Tower).ThenInclude(t => t.Tenancies)
                       .FirstOrDefaultAsync(s => s.Id == siteId)
                   ?? throw new NotFoundException($"Site {siteId} was not found.");

        var op = await db.Operators.FindAsync(request.OperatorId)
                 ?? throw new NotFoundException($"Operator {request.OperatorId} was not found.");

        var tower = site.Tower;

        if (site.Status != SiteStatus.Active)
            throw new BusinessRuleException("Tenants can only be added to active sites.");

        if (!tower.HasCapacity)
            throw new BusinessRuleException($"Tower is full ({tower.ActiveTenantCount}/{tower.MaxTenants} tenants).");

        if (tower.Tenancies.Any(t => t.IsActive && t.OperatorId == op.Id))
            throw new BusinessRuleException($"{op.Name} is already a tenant on this tower.");

        var tenancy = new Tenancy
        {
            Operator = op,
            StartDate = request.StartDate,
            MonthlyRent = request.MonthlyRent
        };
        tower.Tenancies.Add(tenancy);
        await db.SaveChangesAsync();

        return tenancy.ToDto();
    }

    public async Task EndAsync(int tenancyId)
    {
        var tenancy = await db.Tenancies.FindAsync(tenancyId)
                      ?? throw new NotFoundException($"Tenancy {tenancyId} was not found.");

        if (!tenancy.IsActive) return;

        tenancy.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
        await db.SaveChangesAsync();
    }
}
