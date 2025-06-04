using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces.Repositories.Write;
public interface IWriteJobAdRepository
{
    public Task<List<JobAd>> InsertJobAds(IEnumerable<JobAdCreateDto> jobAds, bool doBulkInsert = false);
    public Task AddDescription(IEnumerable<DescriptionCreateDto> descriptions);
}
