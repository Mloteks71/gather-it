using Domain.Entities;

namespace Application.Interfaces.Repositories.Read;
public interface IReadJobAdRepository
{
    public IEnumerable<JobAd> GetJobAds(IEnumerable<string> companyNames);
    public IEnumerable<string> GetJobAdsSlug();
}
