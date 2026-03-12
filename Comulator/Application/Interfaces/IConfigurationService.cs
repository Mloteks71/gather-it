using Domain.Enums;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace Application.Interfaces;

public interface IConfigurationService
{
    string JustJoinItUrl { get; }
    string TheProtocolItUrl { get; }
    string SolidJobsUrl { get; }
    string RabbitMqHostName { get; }
    int RabbitMqPort { get; }
    string RabbitMqUserName { get; }
    string RabbitMqPassword { get; }
    string RabbitMqExchangeName { get; }
    RabbitMqServiceOptions RabbitMqServiceOptions { get; }
    Dictionary<Site, string> RabbitMqDescriptionServiceRoutingKeys { get; }
    string RabbitMqMappingRoutingKey { get; }
    string RabbitMqMappingExchangeName { get; }
}
