using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IComulator
{
    public Task<IEnumerable<CommonJobAdDto>> Comulate();
}
