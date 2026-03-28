using System.Text;
using Application.Interfaces;
using Application.Interfaces.MessageSenders;
using Application.Models.Dtos;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace Application.Services.MessageSenders;

public class MappingServiceMessageSender : IMappingServiceMessageSender
{
    private readonly IProducingService _producingService;
    private readonly string _exchangeName;
    private readonly string _routingKey;
    private readonly ILogger<IMappingServiceMessageSender> _logger;

    public MappingServiceMessageSender(
        IConfigurationService config,
        IProducingService producingService,
        ILogger<IMappingServiceMessageSender> logger)
    {
        _producingService = producingService;
        _exchangeName = config.RabbitMqMappingExchangeName;
        _routingKey = config.RabbitMqMappingRoutingKey;
        _logger = logger;
    }

    public async Task SendMappedJobAdsAsync(List<CommonJobAdDto> jobAds)
    {
        if (jobAds == null || jobAds.Count == 0)
        {
            _logger.LogWarning("No job ads to send to mapping queue");
            return;
        }

        _logger.LogInformation(
            "Sending {Count} mapped job ads to RabbitMQ",
            jobAds.Count);

        await _producingService.SendAsync(jobAds, _exchangeName, _routingKey);

        _logger.LogInformation("Successfully sent {Count} job ads to mapping queue", jobAds.Count);
    }
}
