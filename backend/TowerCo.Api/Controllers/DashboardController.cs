using Microsoft.AspNetCore.Mvc;
using TowerCo.Api.Dtos;
using TowerCo.Api.Services;

namespace TowerCo.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet]
    public async Task<DashboardDto> Get() => await dashboard.GetAsync();
}
