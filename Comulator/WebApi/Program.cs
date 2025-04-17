using Application.Interfaces;
using Application.Interfaces.HttpClients;
using Application.Interfaces.MessageSenders;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Read;
using Application.Interfaces.Repositories.Write;
using Domain;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.HttpClients;
using Infrastructure.Services.MessageSenders;
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

builder.Services.AddScoped<IWriteJobAdRepository, WriteJobRepository>();
builder.Services.AddScoped<IReadJobAdRepository, ReadJobRepository>();
builder.Services.AddScoped<ICompanyNameRepository, CompanyNameRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddScoped<IDocumentSimilarityService, DocumentSimilarityService>();
builder.Services.AddScoped<IJobAdService, JobAdService>();
builder.Services.AddScoped<IComulator, Comulator>();
builder.Services.AddScoped<IDescriptionServiceMessageSender, DescriptionServiceMessageSender>();

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

await using (var serviceScope = app.Services.CreateAsyncScope())
using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<GatherItDbContext>())
{
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
