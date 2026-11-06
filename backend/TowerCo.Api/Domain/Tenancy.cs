namespace TowerCo.Api.Domain;

// A lease of space on a tower by an operator. Ending a lease keeps the history.
public class Tenancy
{
    public int Id { get; set; }
    public int TowerId { get; set; }
    public int OperatorId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal MonthlyRent { get; set; }

    public bool IsActive => EndDate is null;

    public Tower Tower { get; set; } = null!;
    public Operator Operator { get; set; } = null!;
}
