using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApi.HostedServices;

public class SchedulerRegistrationService : BackgroundService
{
    private const int RetryLimit = 5;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(15);
    private readonly IConfiguration _configuration;
    private readonly ILogger<SchedulerRegistrationService> _logger;

    public SchedulerRegistrationService(
        IConfiguration configuration,
        ILogger<SchedulerRegistrationService> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var schedulerUrl = _configuration["Scheduler:Url"]!;
        var payload = new RegisterWorkerRequest
        {
            Id = _configuration["Scheduler:WorkerId"]!,
            Endpoint = _configuration["Scheduler:WorkerEndpoint"]!,
            Interval = Convert.ToInt32(_configuration["Scheduler:IntervalSeconds"]!),
        };

        for (var attempt = 1; attempt <= RetryLimit; attempt++)
        {
            if (ct.IsCancellationRequested)
                return;

            try
            {
                using var client = new HttpClient();
                var response = await client.PostAsJsonAsync(schedulerUrl, payload, ct);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Successfully registered with scheduler at {Url}",
                        schedulerUrl
                    );
                    return;
                }

                _logger.LogWarning(
                    "Attempt {Attempt}/{RetryLimit} - Scheduler returned {StatusCode}",
                    attempt,
                    RetryLimit,
                    response.StatusCode
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "Attempt {Attempt}/{RetryLimit} - Failed to reach scheduler: {Message}",
                    attempt,
                    RetryLimit,
                    ex.Message
                );
            }

            if (attempt < RetryLimit)
                await Task.Delay(RetryDelay, ct);
        }

        _logger.LogError(
            "Exceeded {RetryLimit} registration attempts. Service will continue without scheduler registration.",
            RetryLimit
        );
    }
}

file sealed class RegisterWorkerRequest
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string Endpoint { get; init; } = string.Empty;

    [JsonPropertyName("interval")]
    public int Interval { get; init; }
}
