using Application.Dtos;
using Application.Dtos.Api.Requests;
using Application.Interfaces.Repositories.Write;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobAdController : BaseController
{
    private readonly IWriteJobAdRepository _writeJobAdRepository;

    public JobAdController(ILogger<JobAdController> logger, IWriteJobAdRepository jobAdRepository) : base(logger)
    {
        _writeJobAdRepository = jobAdRepository;
    }

    [HttpPost("Description")]
    public async Task<ActionResult> PostJobAdDescription(List<AddDescriptionRequestDto> requestDto)
    {
        if (requestDto == null || requestDto.Count == 0)
        {
            return BadRequest("Request DTO cannot be null or empty.");
        }

        Logger.LogInformation("Received {Count} job ad descriptions to add. xD", requestDto.Count);

        if (requestDto.Any(x => x.Id <= 0))
        {
            return BadRequest("All job ad IDs must be greater than zero.");
        }

        await _writeJobAdRepository.AddDescription(requestDto.Select(x => new DescriptionCreateDto(x.Id, x.Description, x.Requirements, x.Benefits, x.Workstyle, x.AboutProject)));

        return Created((string?)null, null);
    }
}
