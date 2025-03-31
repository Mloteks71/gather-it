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

    public ComulatorController(IComulator comulator, IJobAdRepository jobAdRepository) {
        _comulator = comulator;
        _jobAdRepository = jobAdRepository;
    }

    [HttpPost("download")]
    public async Task<ActionResult> DownloadJobData() {
        var jobAdsToAdd = (await _comulator.Comulate()).ToList();

        var jobAdsFromDb = _jobAdRepository.GetJobAds(jobAdsToAdd.Select(x => x.CompanyName!.Name));

        //jeżeli 99% oznaczyć i do tabelki wspólnej var similarityArray = _documentSimilarityService.CalculateSimilarity(jobAds.Select(x => x.Description!).ToList(), jobAdsFromDb.Select(x => x.Description!).ToList()); // nie mam description xD
        RemoveDuplicateJobAdsTempImplementation(jobAdsToAdd, jobAdsFromDb);

        await _jobAdRepository.InsertJobAds(jobAdsToAdd);

        return Ok();
    }

    private static void RemoveDuplicateJobAdsTempImplementation(IEnumerable<JobAdCreateDto> jobAdsToAdd, IEnumerable<JobAd> jobAdsFromDb) => 
        jobAdsToAdd.ToList().RemoveAll(x => jobAdsFromDb.Any(y => y.Slug == x.Slug));
}
