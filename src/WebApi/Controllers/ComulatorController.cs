using Application.Dtos;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComulatorController(IComulator comulator, IJobAdRepository jobAdRepository, ICityRepository cityRepository, ICompanyNameRepository companyNameRepository, IDocumentSimilarityService documentSimilarityService) : ControllerBase
{
    private readonly IComulator _comulator = comulator;
    private readonly IJobAdRepository _jobAdRepository = jobAdRepository;
    private readonly ICityRepository _cityRepository = cityRepository;
    private readonly ICompanyNameRepository _companyNameRepository = companyNameRepository;
    private readonly IDocumentSimilarityService _documentSimilarityService = documentSimilarityService;

    [HttpPost("download")]
    public async Task<ActionResult> DownloadJobData() {
        var jobAds = (await _comulator.Comulate()).ToList();

        var jobAdsFromDb = _jobAdRepository.GetJobAds(jobAds.Select(x => x.CompanyName!.Name));

        //jeżeli 99% oznaczyć i do tabelki wspólnej var similarityArray = _documentSimilarityService.CalculateSimilarity(jobAds.Select(x => x.Description!).ToList(), jobAdsFromDb.Select(x => x.Description!).ToList()); // nie mam description xD
        RemoveDuplicateJobAdsTempImplementation(jobAds, jobAdsFromDb);

        await _jobAdRepository.InsertJobAds(jobAds);

        return Ok();
    }

    private static void RemoveDuplicateJobAdsTempImplementation(IEnumerable<JobAdCreateDto> jobAds, IEnumerable<JobAd> jobAdsFromDb) => 
        jobAds.ToList().RemoveAll(x => jobAdsFromDb.Any(y => y.Slug == x.Slug));
}
