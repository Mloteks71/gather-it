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
    public async Task<ActionResult> DownloadJobData()
    {
        List<JobAd> jobAds = await _comulator.Comulate();

        var jobAdsFromDb = _jobAdRepository.GetJobAds(jobAds.Select(x => x.CompanyName!.Name));

        //jeżeli 99% oznaczyć i do tabelki wspólnej var similarityArray = _documentSimilarityService.CalculateSimilarity(jobAds.Select(x => x.Description!).ToList(), jobAdsFromDb.Select(x => x.Description!).ToList()); // nie mam description xD

        RemoveDuplicateJobAdsTempImplementation(jobAds, jobAdsFromDb);

        List<City> cities = _cityRepository.GetCities().OrderBy(x => x.Name).ToList();
        IOrderedEnumerable<CompanyName?> companyNames = _companyNameRepository.GetCompanyNames().OrderBy(x => x!.Name); ;

        jobAds = StandarizeCities(jobAds, cities);
        StandarizeCompanyNames(jobAds, companyNames!);

        await _jobAdRepository.InsertJobAds(jobAds);

        return Ok();
    }

    private static void RemoveDuplicateJobAdsTempImplementation(List<JobAd> jobAds, IEnumerable<JobAd> jobAdsFromDb)
    {
        Queue<JobAd> queue = new(jobAds);

        while (queue.Count > 0)
        {
            var jobAd = queue.Dequeue();
            if (jobAdsFromDb.Any(x => x.Slug == jobAd.Slug))
            {
                jobAds.Remove(jobAd);
            }
        }
    }

    private static void StandarizeCompanyNames(List<JobAd> justJoinItJobs, IOrderedEnumerable<CompanyName> companyNames)
    {
        PriorityQueue<JobAd, string> jobAdQueueForCompanyNames = new();
        //jobAdQueueForCities.EnqueueRange(justJoinItJobs.Where(x => !(x.Cities ?? Enumerable.Empty<City>()).Contains(null)).SelectMany(jobAd => jobAd.Cities ?? Enumerable.Empty<City>(), (jobAd, city) => (jobAd, city.Name!)));
        //jobAdQueueForCities.EnqueueRange(justJoinItJobs.SelectMany(x => (x, x?.Cities?.Select(y => y.Name)))); - sprawdzić, czemu nie działa
        jobAdQueueForCompanyNames.EnqueueRange(justJoinItJobs.Select(jobAd => (jobAd, jobAd.CompanyName!.Name)));
        jobAdQueueForCompanyNames.TryDequeue(out JobAd? jobAdFromQueue, out string? companyNameNameFromQueue);
        foreach (CompanyName companyName in companyNames)
        {
            while (companyNameNameFromQueue == companyName.Name)
            {
                if (companyName.Name == companyNameNameFromQueue)
                {
                    jobAdFromQueue!.CompanyName = companyName;
                }

                // w pętli, dopóki miasto się zgadza w priority queue
                // policzyć ile razy ma być każde miasto, zapisać w tabeli pętla lecąca po tabeli i pętla w środku tyle razy ile ma być
                if (jobAdQueueForCompanyNames.TryDequeue(out jobAdFromQueue, out companyNameNameFromQueue))
                {
                    break;
                }
            }
        }
    }

    private static List<JobAd> StandarizeCities(List<JobAd> justJoinItJobs, List<City> cities)
    {
        PriorityQueue<JobAd, string> jobAdQueueForCities = new();
        jobAdQueueForCities.EnqueueRange(justJoinItJobs.Where(x => !(x.Cities ?? Enumerable.Empty<City>()).Contains(null)).SelectMany(jobAd => jobAd.Cities ?? Enumerable.Empty<City>(), (jobAd, city) => (jobAd, city.Name!)));
        //jobAdQueueForCities.EnqueueRange(justJoinItJobs.SelectMany(x => (x, x?.Cities?.Select(y => y.Name)), y => y)); - sprawdzić, czemu nie działa
        jobAdQueueForCities.TryDequeue(out JobAd? jobAdFromQueue, out string? cityNameFromQueue);
        cities.AddRange(justJoinItJobs.SelectMany(x => x.Cities ?? Enumerable.Empty<City>()).Distinct().Where(x => !cities.Contains(x)));
        foreach (City city in cities)
        {
            while (cityNameFromQueue == city.Name)
            {
                jobAdFromQueue?.Cities?.RemoveAll(x => x.Name == city.Name);
                jobAdFromQueue?.Cities?.Add(city);

                // w pętli, dopóki miasto się zgadza w priority queue
                // policzyć ile razy ma być każde miasto, zapisać w tabeli pętla lecąca po tabeli i pętla w środku tyle razy ile ma być
                if (!jobAdQueueForCities.TryDequeue(out jobAdFromQueue, out cityNameFromQueue))
                {
                    break;
                }
            }
        }
        return justJoinItJobs;
    }
}
