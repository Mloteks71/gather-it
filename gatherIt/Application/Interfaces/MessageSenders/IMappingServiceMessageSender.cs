using Application.Models.Dtos;

namespace Application.Interfaces.MessageSenders;

public interface IMappingServiceMessageSender
{
    Task SendMappedJobAdsAsync(List<CommonJobAdDto> jobAds);
}
