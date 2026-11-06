using Microsoft.EntityFrameworkCore;
using TowerCo.Api.Domain;

namespace TowerCo.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Tower> Towers => Set<Tower>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<Tenancy> Tenancies => Set<Tenancy>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Site>(e =>
        {
            e.HasIndex(s => s.Code).IsUnique();
            e.Property(s => s.Code).HasMaxLength(20).IsRequired();
            e.Property(s => s.Name).HasMaxLength(100).IsRequired();
            e.Property(s => s.Country).HasMaxLength(60).IsRequired();
            e.Property(s => s.City).HasMaxLength(60);
            e.Property(s => s.Status).HasConversion<string>();
            e.HasOne(s => s.Tower).WithOne(t => t.Site)
                .HasForeignKey<Tower>(t => t.SiteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Tower>(e =>
        {
            e.Property(t => t.Type).HasConversion<string>();
            e.Property(t => t.PowerSource).HasConversion<string>();
        });

        model.Entity<Operator>(e =>
        {
            e.HasIndex(o => o.Name).IsUnique();
            e.Property(o => o.Name).HasMaxLength(100).IsRequired();
        });

        model.Entity<Tenancy>(e =>
        {
            // SQLite has no native decimal type, store as double for querying.
            e.Property(t => t.MonthlyRent).HasConversion<double>();
            e.HasOne(t => t.Tower).WithMany(t => t.Tenancies).HasForeignKey(t => t.TowerId);
            e.HasOne(t => t.Operator).WithMany(o => o.Tenancies).HasForeignKey(t => t.OperatorId);
        });
    }
}
