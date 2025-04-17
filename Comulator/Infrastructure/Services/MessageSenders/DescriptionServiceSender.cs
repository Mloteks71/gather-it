using System.Text;
using Application.Dtos.Messages.Requests;
using Application.Interfaces.MessageSenders;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.Services.MessageSenders;
public class DescriptionServiceMessageSender : IDescriptionServiceMessageSender
{
    private readonly string _HostName;
    private readonly Dictionary<Site, string> _RoutingKeys;
    public DescriptionServiceMessageSender(IConfiguration config)
    {
        _HostName = config["RabbitMQ:HostName"]!;
        _RoutingKeys = config.GetSection("RabbitMQ:DescriptionServiceRoutingKeys")
                    .GetChildren()
                    .ToDictionary(x => (Site)Enum.Parse(typeof(Site), x.Key), x => x.Value!);
    }

    public virtual async Task SendDescriptionRequestList(ILookup<Site, DescriptionRequestDto> descriptionRequestDtoLookup)
    {
        var factory = new ConnectionFactory { HostName = _HostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var props = new BasicProperties();

        foreach (var descriptionRequestDtoList in descriptionRequestDtoLookup)
        {
            var routingKey = _RoutingKeys[descriptionRequestDtoList.Key];
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(descriptionRequestDtoList.ToString()!);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: routingKey, mandatory: false, basicProperties: props, body: messageBodyBytes);
        }
    }
}
