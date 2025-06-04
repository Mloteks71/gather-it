using Application.Interfaces.Repositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories;
public class ReadCityRepository(GatherItDbContext context) : IReadCityRepository
{
    private readonly GatherItDbContext _context = context;

    public IEnumerable<City> GetCities()
    {
        return _context.Cities;
    }
}
