using Application.Interfaces.Repositories.Read;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories.Read;
public class ReadCompanyNameRepository(GatherItDbContext context) : IReadCompanyNameRepository
{
    private readonly GatherItDbContext _context = context;

    public IEnumerable<CompanyName> GetCompanyNames()
    {
        return _context.CompanyNames;
    }
}
