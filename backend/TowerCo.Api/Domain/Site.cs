namespace TowerCo.Api.Domain;

// The location we own or lease the land for: where it is and its operational status.
public class Site
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public SiteStatus Status { get; set; }
    public DateOnly? CommissionedOn { get; set; }

    public Tower Tower { get; set; } = null!;
}
