using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace Application.Services;

public class ConfigurationService : IConfigurationService
{
    public string JustJoinItUrl { get; }
    public string TheProtocolItUrl { get; }
    public string SolidJobsUrl { get; }
    public string PostgresConnectionString { get; }
    public string RabbitMqHostName { get; }
    public int RabbitMqPort { get; }
    public string RabbitMqUserName { get; }
    public string RabbitMqPassword { get; }
    public string RabbitMqExchangeName { get; }
    public RabbitMqServiceOptions RabbitMqServiceOptions => new()
    {
        HostName = RabbitMqHostName,
        Port = RabbitMqPort,
        UserName = RabbitMqUserName,
        Password = RabbitMqPassword,
    };
    public Dictionary<Site, string> RabbitMqDescriptionServiceRoutingKeys { get; }

    public ConfigurationService(IConfiguration configuration)
    {
        JustJoinItUrl = configuration["JustJoinIt:Url"]!;
        TheProtocolItUrl = configuration["TheProtocolIt:Url"]!;
        SolidJobsUrl = configuration["SolidJobs:Url"]!;

        var dbConfig = configuration.GetSection("Database:PostgreSQL");
        PostgresConnectionString =
            $"Host={dbConfig["Server"]};Database={dbConfig["Database"]};Username={dbConfig["UserId"]};Password={dbConfig["Password"]};Include Error Detail=true;";

        var rabbitSection = configuration.GetSection("RabbitMQ");
        RabbitMqHostName = rabbitSection["HostName"]!;
        RabbitMqPort = Convert.ToInt32(rabbitSection["Port"]!);
        RabbitMqUserName = rabbitSection["UserName"]!;
        RabbitMqPassword = rabbitSection["Password"]!;
        RabbitMqExchangeName = rabbitSection["Exchange:Name"]!;

        RabbitMqDescriptionServiceRoutingKeys = rabbitSection.GetSection("DescriptionServiceRoutingKeys")
            .GetChildren()
            .ToDictionary(x => (Site)Enum.Parse(typeof(Site), x.Key), x => x.Value!);
    }
}
