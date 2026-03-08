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

        const int batchSize = 50;
        var batches = jobAds.Chunk(batchSize).ToList();

        _logger.LogInformation(
            "Sending {TotalCount} mapped job ads to RabbitMQ in {BatchCount} batches",
            jobAds.Count,
            batches.Count);

        for (int i = 0; i < batches.Count; i++)
        {
            var batch = batches[i].ToList();
            await _producingService.SendAsync(batch, _exchangeName, _routingKey);
            
            _logger.LogInformation(
                "Sent batch {BatchNumber}/{TotalBatches} ({Count} job ads) to mapping queue",
                i + 1,
                batches.Count,
                batch.Count);
        }

        _logger.LogInformation("Successfully sent all {Count} job ads to mapping queue", jobAds.Count);
    }
}
