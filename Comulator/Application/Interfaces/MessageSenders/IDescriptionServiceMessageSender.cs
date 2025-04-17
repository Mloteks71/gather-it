using Application.Dtos.Messages.Requests;
using Domain.Enums;

namespace Application.Interfaces.MessageSenders;
public interface IDescriptionServiceMessageSender
{
    Task SendDescriptionRequestList(ILookup<Site, DescriptionRequestDto> descriptionRequestDtoLookup);
}
