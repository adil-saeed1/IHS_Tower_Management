namespace TowerCo.Api.Domain;

// The physical structure on a site. MaxTenants is the leasing capacity.
public class Tower
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public TowerType Type { get; set; }
    public double HeightMeters { get; set; }
    public int MaxTenants { get; set; }
    public PowerSource PowerSource { get; set; }

    public Site Site { get; set; } = null!;
    public List<Tenancy> Tenancies { get; set; } = new();

    public int ActiveTenantCount => Tenancies.Count(t => t.IsActive);
    public int AvailableSlots => Math.Max(0, MaxTenants - ActiveTenantCount);
    public bool HasCapacity => AvailableSlots > 0;
    public double UtilisationPercent => MaxTenants == 0 ? 0 : Math.Round(100.0 * ActiveTenantCount / MaxTenants, 1);
}
