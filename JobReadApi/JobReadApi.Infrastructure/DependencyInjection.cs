using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using JobReadApi.Application.Enums;
using JobReadApi.Application.Interfaces;
using JobReadApi.Infrastructure.Data;
using JobReadApi.Infrastructure.Repositories;

namespace JobReadApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReadApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");

        Console.WriteLine("=== REGISTERING ENUM MAPPINGS ===");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<JobSite>();
        dataSourceBuilder.MapEnum<ContractType>();
        dataSourceBuilder.MapEnum<WorkplaceType>();
        dataSourceBuilder.MapEnum<ExperienceLevel>();
        dataSourceBuilder.MapEnum<OfferStatus>();
        Console.WriteLine("=== ENUM MAPPINGS REGISTERED ===");
        var dataSource = dataSourceBuilder.Build();

        // Register the data source as a singleton
        services.AddSingleton(dataSource);

        services.AddDbContext<ReadApiDbContext>((serviceProvider, options) =>
        {
            var ds = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            options.UseNpgsql(ds, npgsqlOptions =>
            {
                // These tell EFCore's type mapper to use NpgsqlEnumTypeMapping
                // instead of the default integer mapping for C# enums
                npgsqlOptions.MapEnum<JobSite>();
                npgsqlOptions.MapEnum<ContractType>();
                npgsqlOptions.MapEnum<WorkplaceType>();
                npgsqlOptions.MapEnum<ExperienceLevel>();
                npgsqlOptions.MapEnum<OfferStatus>();
            });
        });

        services.AddScoped<IJobAdReadRepository, JobAdReadRepository>();
        services.AddScoped<ILookupReadRepository, LookupReadRepository>();

        return services;
    }
}

