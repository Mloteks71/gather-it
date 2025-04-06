using System.Diagnostics;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComulatorController : BaseController
{
    private readonly IComulator _comulator;
    private readonly IJobAdRepository _jobAdRepository;
    private readonly IJobAdService _jobAdService;

    public ComulatorController(
        IComulator comulator,
        IJobAdRepository jobAdRepository,
        IJobAdService jobAdService,
        ILogger<ComulatorController> logger) : base(logger)
    {
        _comulator = comulator;
        _jobAdRepository = jobAdRepository;
        _jobAdService = jobAdService;
    }

    [HttpPost("download")]
    public async Task<ActionResult> DownloadJobData()
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var comulatedJobAds = (await _comulator.Comulate()).ToList();

        var jobAdSlugsFromDatabase = _jobAdRepository.GetJobAdsSlug().ToHashSet();

        var filteredJobAdsToAdd = _jobAdService.RemoveJobAds(comulatedJobAds, x => !jobAdSlugsFromDatabase.Contains(x.Slug));

        await _jobAdRepository.InsertJobAds(filteredJobAdsToAdd);

        stopWatch.Stop();
        Logger.LogInformation("Downloaded and inserted {JobAdsInserted} JobAds into the database. Time elapsed: {ElapsedMilliseconds} ms", comulatedJobAds.Count, stopWatch.ElapsedMilliseconds);
        return Ok();
    }
}
