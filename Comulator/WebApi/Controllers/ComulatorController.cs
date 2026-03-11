using System.Diagnostics;
using Application.Dtos.Messages.Requests;
using Application.Interfaces;
using Application.Interfaces.MessageSenders;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComulatorController : BaseController
{
    private readonly IComulator _comulator;
    private readonly IMappingServiceMessageSender _mappingServiceMessageSender;

    public ComulatorController(
        IComulator comulator,
        ILogger<ComulatorController> logger,
        IMappingServiceMessageSender mappingServiceMessageSender) : base(logger)
    {
        _comulator = comulator;
        _mappingServiceMessageSender = mappingServiceMessageSender;
    }

    [HttpPost("Download")]
    public async Task<ActionResult> DownloadJobData(bool getDescription = true)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var comulatedJobAds = (await _comulator.Comulate()).ToList();

        stopWatch.Stop();
        Logger.LogInformation("Downloaded {JobAdsInserted} JobAds. Time elapsed: {ElapsedMilliseconds} ms", comulatedJobAds.Count, stopWatch.ElapsedMilliseconds);
        
        await _mappingServiceMessageSender.SendMappedJobAdsAsync(comulatedJobAds);
        
        return Ok();
    }
}
