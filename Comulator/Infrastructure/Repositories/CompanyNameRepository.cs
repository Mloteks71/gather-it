using Application.Interfaces.Repositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories;
public class CompanyNameRepository(GatherItDbContext context) : ICompanyNameRepository
{
    private readonly GatherItDbContext _context = context;

    public IEnumerable<CompanyName> GetCompanyNames()
    {
        return _context.CompanyNames;
    }
}
