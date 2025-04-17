using Application.Dtos;

namespace Application.Interfaces;

public interface IComulator
{
    public Task<IEnumerable<JobAdCreateDto>> Comulate();
}
