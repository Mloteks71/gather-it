using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Interfaces.MessageSenders;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Read;
using Application.Interfaces.Repositories.Write;
using Domain;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Read;
using Infrastructure.Repositories.Write;
using Infrastructure.Services;
using Infrastructure.Services.HttpClients;
using Infrastructure.Services.MessageSenders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using Serilog;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var postgresConnectionString = DatabaseConfigHelper.GetPostgresConnectionString(configuration);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GatherItDbContext>(options =>
    options.UseNpgsql(postgresConnectionString,
        optionsBuilder => optionsBuilder.MigrationsAssembly("Domain")));

builder.Services.AddScoped<IWriteJobAdRepository, WriteJobRepository>();
builder.Services.AddScoped<IReadJobAdRepository, ReadJobRepository>();
builder.Services.AddScoped<IReadCompanyNameRepository, ReadCompanyNameRepository>();
builder.Services.AddScoped<IReadCityRepository, ReadCityRepository>();

builder.Services.AddScoped<IDocumentSimilarityService, DocumentSimilarityService>();
builder.Services.AddScoped<IJobAdService, JobAdService>();
builder.Services.AddScoped<IComulator, Comulator>();
builder.Services.AddScoped<IDescriptionServiceMessageSender, DescriptionServiceMessageSender>();

builder.Services.AddHttpClient<IJustJoinItHttpClient, JustJoinItHttpClient>();
builder.Services.AddHttpClient<ITheProtocolItHttpClient, TheProtocolItHttpClient>();
builder.Services.AddHttpClient<ISolidJobsHttpClient, SolidJobsHttpClient>();

var rabbitSection = configuration.GetSection("RabbitMQ");
var rabbitMQConfiguration = new RabbitMqServiceOptions
{
    HostName = rabbitSection["HostName"]!,
    Port = Convert.ToInt32(rabbitSection["Port"]!),
    UserName = rabbitSection["UserName"]!,
    Password = rabbitSection["Password"]!
};
builder.Services.AddRabbitMqServices(rabbitMQConfiguration);

builder.Services.AddProductionExchange(rabbitSection["Exchange:Name"]!, new RabbitMqExchangeOptions());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

await using (var serviceScope = app.Services.CreateAsyncScope())
await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<GatherItDbContext>())
{
    dbContext.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
