using System.Diagnostics;
using Application.Dtos.Messages.Requests;
using Application.Interfaces;
using Application.Interfaces.MessageSenders;
using Application.Interfaces.Repositories.Read;
using Application.Interfaces.Repositories.Write;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComulatorController : BaseController
{
    private readonly IComulator _comulator;
    private readonly IReadJobAdRepository _readJobAdRepository;
    private readonly IWriteJobAdRepository _writeJobAdRepository;
    private readonly IJobAdService _jobAdService;
    private readonly IDescriptionServiceMessageSender _descriptionServiceMessageSender;

    public ComulatorController(
        IComulator comulator,
        IReadJobAdRepository readJobAdRepository,
        IWriteJobAdRepository writeJobAdRepository,
        IJobAdService jobAdService,
        ILogger<ComulatorController> logger,
        IDescriptionServiceMessageSender descriptionServiceMessageSender) : base(logger)
    {
        _comulator = comulator;
        _writeJobAdRepository = writeJobAdRepository;
        _readJobAdRepository = readJobAdRepository;
        _jobAdService = jobAdService;
        _descriptionServiceMessageSender = descriptionServiceMessageSender;
    }

    [HttpPost("Download")]
    public async Task<ActionResult> DownloadJobData(bool getDescription = true)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var comulatedJobAds = (await _comulator.Comulate()).ToList();

        var jobAdSlugsFromDatabase = _readJobAdRepository.GetJobAdsSlug().ToHashSet();

        var filteredJobAdsToAdd = _jobAdService.RemoveJobAds(comulatedJobAds, x => !jobAdSlugsFromDatabase.Contains(x.Slug));

        var insertedJobAds = await _writeJobAdRepository.InsertJobAds(filteredJobAdsToAdd/*, true*/);

        if (getDescription)
        {
            await _descriptionServiceMessageSender.SendDescriptionRequestList(insertedJobAds.ToLookup(x => x.Site, x => new DescriptionRequestDto { Id = x.Id, Slug = x.Slug }));
        }

        stopWatch.Stop();
        Logger.LogInformation("Downloaded and inserted {JobAdsInserted} JobAds into the database. Time elapsed: {ElapsedMilliseconds} ms", comulatedJobAds.Count, stopWatch.ElapsedMilliseconds);
        return Ok();
    }
}
