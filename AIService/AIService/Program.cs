using AIService;
using AIService.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure RabbitMQ
builder.Services.Configure<RabbitMQConfiguration>(
    builder.Configuration.GetSection("RabbitMQ"));

// Configure AI Service
builder.Services.Configure<AIConfiguration>(
    builder.Configuration.GetSection("AI"));

// Register HttpClient for AI Service
builder.Services.AddHttpClient<IAIService, OpenAIService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AIConfiguration>>().Value;
    client.BaseAddress = new Uri(config.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(120);
});

// Register Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
