using Application.Models.Dtos;
using Application.Models.Responses;

namespace Application.Interfaces;

public interface IResponseMapper
{
    IEnumerable<CommonJobAdDto> MapJustJoinItResponse(JustJoinItResponse response);
    IEnumerable<CommonJobAdDto> MapTheProtocolItResponse(TheProtocolItResponse response);
}
