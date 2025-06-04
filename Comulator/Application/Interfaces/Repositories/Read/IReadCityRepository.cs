using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IReadCityRepository
{
    public IEnumerable<City> GetCities();
}
