using Microsoft.AspNetCore.Mvc;
using TowerCo.Api.Dtos;
using TowerCo.Api.Services;

namespace TowerCo.Api.Controllers;

[ApiController]
[Route("api/operators")]
public class OperatorsController(ITenancyService tenancies) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<OperatorDto>> GetAll() => await tenancies.GetOperatorsAsync();
}
