using Microsoft.AspNetCore.Mvc;
using TowerCo.Api.Services;

namespace TowerCo.Api.Controllers;

[ApiController]
[Route("api/tenancies")]
public class TenanciesController(ITenancyService tenancies) : ControllerBase
{
    // Ends the lease rather than deleting it, so tenancy history is kept.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> End(int id)
    {
        await tenancies.EndAsync(id);
        return NoContent();
    }
}
