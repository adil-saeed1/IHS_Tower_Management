namespace TowerCo.Api.Domain;

// A mobile network operator that leases space on our towers.
public class Operator
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Tenancy> Tenancies { get; set; } = new();
}
