using System.Text;
using Application.Dtos.Messages.Requests;
using Application.Interfaces.MessageSenders;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace Infrastructure.Services.MessageSenders;
public class DescriptionServiceMessageSender : IDescriptionServiceMessageSender
{
    private readonly Dictionary<Site, string> _routingKeys;
    private readonly IProducingService _producingService;
    private readonly IConfiguration _configuration;
    private readonly ILogger Logger;

    public DescriptionServiceMessageSender(IConfiguration config, IProducingService producingService, ILogger<IDescriptionServiceMessageSender> logger)
    {
        _routingKeys = config.GetSection("RabbitMQ:DescriptionServiceRoutingKeys")
                    .GetChildren()
                    .ToDictionary(x => (Site)Enum.Parse(typeof(Site), x.Key), x => x.Value!);
        _producingService = producingService;
        _configuration = config;
        Logger = logger;
    }

    public async Task SendDescriptionRequestList(ILookup<Site, DescriptionRequestDto> descriptionRequestDtoLookup)
    {
        foreach (IGrouping<Site, DescriptionRequestDto> descriptionRequestDtoList in descriptionRequestDtoLookup)
        {
            if (descriptionRequestDtoList.Key == Site.SolidJobs)
            {
                continue;
            }

            var routingKey = _routingKeys[descriptionRequestDtoList.Key];
            var exchangeName = _configuration["RabbitMQ:Exchange:Name"]!;
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(descriptionRequestDtoList.ToList().ToString()!);

            Logger.LogInformation("RoutingKey {routingKey}, exchangeName {exchangeName}", routingKey, exchangeName);

            await _producingService.SendAsync(descriptionRequestDtoList.ToList(), exchangeName, routingKey);
        }
    }
}
