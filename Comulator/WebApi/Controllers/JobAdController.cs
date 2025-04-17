
using System.Collections.Frozen;
using Application.Dtos.Api.Requests;
using Application.Interfaces.Repositories.Write;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobAdController : BaseController
{
    private readonly IWriteJobAdRepository _writeJobAdRepository;

    public JobAdController(ILogger logger, IWriteJobAdRepository jobAdRepository) : base(logger)
    {
        _writeJobAdRepository = jobAdRepository;
    }

    [HttpPost("Description")]
    public async Task<ActionResult> GetJobAdDescription(List<AddDescriptionRequestDto> requestDto)
    {
        await _writeJobAdRepository.AddDescription(requestDto.ToFrozenDictionary(x => x.Id, x => x.Description));

        return NoContent();
    }
}
