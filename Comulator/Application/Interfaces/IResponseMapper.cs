using Application.Models.Dtos;
using Application.Models.Responses;

namespace Application.Interfaces;

public interface IResponseMapper
{
    List<CommonJobAdDto> MapJustJoinItResponse(JustJoinItResponse response);
    List<CommonJobAdDto> MapTheProtocolItResponse(TheProtocolItResponse response);
}
