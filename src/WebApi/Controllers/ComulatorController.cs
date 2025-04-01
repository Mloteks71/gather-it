using System.Diagnostics;
using Application.Dtos;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComulatorController : ControllerBase
{
    private readonly IComulator _comulator;
    private readonly IJobAdRepository _jobAdRepository;
    private readonly IJobAdService _jobAdService;

    public ComulatorController(IComulator comulator, IJobAdRepository jobAdRepository, IJobAdService jobAdService) {
        _comulator = comulator;
        _jobAdRepository = jobAdRepository;
        _jobAdService = jobAdService;
    }

    [HttpPost("download")]
    public async Task<ActionResult> DownloadJobData() {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var comulatedJobAds = (await _comulator.Comulate()).ToList();

        var jobAdSlugsFromDatabase = _jobAdRepository.GetJobAdsSlug().ToHashSet();

        var filteredJobAdsToAdd = _jobAdService.RemoveDuplicateJobAds(comulatedJobAds, x => !jobAdSlugsFromDatabase.Contains(x.Slug));

        await _jobAdRepository.InsertJobAds(filteredJobAdsToAdd);

        stopWatch.Stop();
        Console.WriteLine(stopWatch.ElapsedMilliseconds);
        return Ok();
    }
}
