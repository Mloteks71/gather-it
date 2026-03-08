using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Interfaces.MessageSenders;
using Application.Services;
using Application.Services.HttpClients;
using Application.Services.MessageSenders;
using Infrastructure.Services.MessageSenders;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var configService = new ConfigurationService(configuration);
builder.Services.AddSingleton<IConfigurationService>(configService);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJobAdService, JobAdService>();
builder.Services.AddScoped<IComulator, Comulator>();
builder.Services.AddScoped<IDescriptionServiceMessageSender, DescriptionServiceMessageSender>();
builder.Services.AddScoped<IMappingServiceMessageSender, MappingServiceMessageSender>();
builder.Services.AddScoped<IResponseMapper, ResponseMapper>();

// Configure HTTP clients with their respective headers
// Headers are set once during client creation, not in constructors
builder.Services.AddHttpClient<IJustJoinItHttpClient, JustJoinItHttpClient>(client =>
{
    var headers = configuration.GetSection("JustJoinIt:HttpHeaders").GetChildren();
    foreach (var header in headers)
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
    }
});

builder.Services.AddHttpClient<ITheProtocolItHttpClient, TheProtocolItHttpClient>(client =>
{
    var headers = configuration.GetSection("TheProtocolIt:HttpHeaders").GetChildren();
    foreach (var header in headers)
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
    }
});

builder.Services.AddRabbitMqServices(configService.RabbitMqServiceOptions);

builder.Services.AddProductionExchange(
    configService.RabbitMqExchangeName,
    new RabbitMqExchangeOptions()
);

builder.Services.AddProductionExchange(
    configService.RabbitMqMappingExchangeName,
    new RabbitMqExchangeOptions()
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
