using Microsoft.AspNetCore.Mvc;
using JobReadApi.Application.Enums;
using JobReadApi.Application.Interfaces;

namespace JobReadApi.WebApi.Controllers;

[ApiController]
[Route("api/job-ads")]
public class JobAdsController(IJobAdReadRepository jobAdReadRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] OfferStatus? status = null,
        [FromQuery] JobSite? site = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await jobAdReadRepository.GetPagedAsync(page, pageSize, status, site, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{jobAdId:int}")]
    public async Task<IActionResult> GetById(int jobAdId, CancellationToken cancellationToken)
    {
        var result = await jobAdReadRepository.GetByIdAsync(jobAdId, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}

