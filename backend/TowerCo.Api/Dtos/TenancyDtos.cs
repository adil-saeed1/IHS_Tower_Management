using System.ComponentModel.DataAnnotations;

namespace TowerCo.Api.Dtos;

public record TenancyDto(
    int Id, int OperatorId, string OperatorName, DateOnly StartDate, DateOnly? EndDate,
    decimal MonthlyRent, bool IsActive);

public record OperatorDto(int Id, string Name);

public class AddTenancyRequest
{
    [Range(1, int.MaxValue)] public int OperatorId { get; set; }
    public DateOnly StartDate { get; set; }
    [Range(0, 1_000_000)] public decimal MonthlyRent { get; set; }
}
