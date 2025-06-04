using Domain.Entities;

namespace Application.Interfaces.Repositories.Read;
public interface IReadCompanyNameRepository
{
    public IEnumerable<CompanyName> GetCompanyNames();
}
