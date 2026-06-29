using Microsoft.AspNetCore.Mvc;
using JobReadApi.Application.Interfaces;

namespace JobReadApi.WebApi.Controllers;

[ApiController]
[Route("api/lookups")]
public class LookupsController(ILookupReadRepository lookupReadRepository) : ControllerBase
{
    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills([FromQuery] string? search = null, [FromQuery] int take = 30, CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 100);
        var result = await lookupReadRepository.GetSkillsAsync(search, take, cancellationToken);
        return Ok(result);
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetCompanies([FromQuery] string? search = null, [FromQuery] int take = 30, CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 100);
        var result = await lookupReadRepository.GetCompaniesAsync(search, take, cancellationToken);
        return Ok(result);
    }
}

