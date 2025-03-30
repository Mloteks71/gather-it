using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces;
public interface IComulator
{
    public Task<IEnumerable<JobAdCreateDto>> Comulate();
}
