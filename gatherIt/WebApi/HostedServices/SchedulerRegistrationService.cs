using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApi.HostedServices;

public class SchedulerRegistrationService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<SchedulerRegistrationService> logger) : BackgroundService
{
    private const int RetryLimit = 5;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(15);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var schedulerUrl = configuration["Scheduler:Url"]!;
        var payload = new RegisterWorkerRequest
        {
            Id = configuration["Scheduler:WorkerId"]!,
            Endpoint = configuration["Scheduler:WorkerEndpoint"]!,
            Interval = Convert.ToInt32(configuration["Scheduler:IntervalSeconds"]!),
        };

        for (var attempt = 1; attempt <= RetryLimit; attempt++)
        {
            if (ct.IsCancellationRequested) return;

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync(schedulerUrl, payload, ct);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Successfully registered with scheduler at {Url}", schedulerUrl);
                    return;
                }

                logger.LogWarning(
                    "Attempt {Attempt}/{RetryLimit} - Scheduler returned {StatusCode}",
                    attempt, RetryLimit, response.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    "Attempt {Attempt}/{RetryLimit} - Failed to reach scheduler: {Message}",
                    attempt, RetryLimit, ex.Message);
            }

            if (attempt < RetryLimit)
                await Task.Delay(RetryDelay, ct);
        }

        logger.LogError(
            "Exceeded {RetryLimit} registration attempts. Service will continue without scheduler registration.",
            RetryLimit);
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
