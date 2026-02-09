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

    public JobAdController(ILogger<JobAdController> logger, IWriteJobAdRepository jobAdRepository)
        : base(logger)
    {
        _writeJobAdRepository = jobAdRepository;
    }

    [HttpPost("Description")]
    public async Task<ActionResult> PostJobAdDescription(List<AddDescriptionRequestDto> requestDto)
    {
        // Validate input is not null or empty
        if (requestDto == null || requestDto.Count == 0)
        {
            return BadRequest(new { error = "Request body cannot be null or empty." });
        }

        // Validate ModelState for data annotations
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Logger.LogInformation("Received {Count} job ad descriptions to add", requestDto.Count);

        try
        {
            await _writeJobAdRepository.AddDescription(
                requestDto.Select(x => new DescriptionCreateDto(
                    x.Id,
                    x.Description,
                    x.Requirements,
                    x.Benefits,
                    x.Workstyle,
                    x.AboutProject
                ))
            );

            var response = new
            {
                message = $"Successfully added {requestDto.Count} job ad descriptions.",
                count = requestDto.Count,
            };
            Logger.LogInformation(
                "Successfully added {Count} job ad descriptions",
                requestDto.Count
            );
            return Accepted(response);
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning("Failed to add descriptions: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while adding job ad descriptions");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = "An unexpected error occurred while processing descriptions." }
            );
        }
    }
}
