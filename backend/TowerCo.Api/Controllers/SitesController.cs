using Microsoft.AspNetCore.Mvc;
using TowerCo.Api.Domain;
using TowerCo.Api.Dtos;
using TowerCo.Api.Services;

namespace TowerCo.Api.Controllers;

[ApiController]
[Route("api/sites")]
public class SitesController(ISiteService sites, ITenancyService tenancies) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<SiteSummaryDto>> Search(
        [FromQuery] string? search, [FromQuery] string? country, [FromQuery] SiteStatus? status) =>
        await sites.SearchAsync(search, country, status);

    [HttpGet("countries")]
    public async Task<IReadOnlyList<string>> Countries() => await sites.GetCountriesAsync();

    [HttpGet("{id:int}")]
    public async Task<SiteDetailDto> Get(int id) => await sites.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<SiteDetailDto>> Create(SaveSiteRequest request)
    {
        var site = await sites.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = site.Id }, site);
    }

    [HttpPut("{id:int}")]
    public async Task<SiteDetailDto> Update(int id, SaveSiteRequest request) => await sites.UpdateAsync(id, request);

    [HttpPost("{id:int}/tenancies")]
    public async Task<ActionResult<TenancyDto>> AddTenancy(int id, AddTenancyRequest request)
    {
        var tenancy = await tenancies.AddAsync(id, request);
        return CreatedAtAction(nameof(Get), new { id }, tenancy);
    }
}
