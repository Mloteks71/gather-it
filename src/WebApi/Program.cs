using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Interfaces.Repositories;
using Domain;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.HttpClients;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddScoped<IJobAdRepository, JobAdRepository>();
builder.Services.AddScoped<ICompanyNameRepository, CompanyNameRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddScoped<IDocumentSimilarityService, DocumentSimilarityService>();
builder.Services.AddScoped<IJobAdService, JobAdService>();
builder.Services.AddScoped<IComulator, Comulator>();

builder.Services.AddHttpClient<IJustJoinItHttpClient, JustJoinItHttpClient>();
builder.Services.AddHttpClient<ITheProtocolItHttpClient, TheProtocolItHttpClient>();
builder.Services.AddHttpClient<ISolidJobsHttpClient, SolidJobsHttpClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
