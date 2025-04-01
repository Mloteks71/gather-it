using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.HttpClients;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var postgresConnectionString = DatabaseConfigHelper.GetPostgresConnectionString(configuration);

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
builder.Services.AddSingleton<IJobAdService, JobAdService>();
builder.Services.AddScoped<IComulator, Comulator>();

builder.Services.AddHttpClient<IJustJoinItJobBoardHttpClient, JustJoinItJobBoardHttpClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
