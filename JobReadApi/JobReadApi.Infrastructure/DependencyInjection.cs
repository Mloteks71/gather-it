using JobReadApi.Application.Enums;
using JobReadApi.Application.Interfaces;
using JobReadApi.Infrastructure.Data;
using JobReadApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace JobReadApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReadApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<JobSite>();
        dataSourceBuilder.MapEnum<ContractType>();
        dataSourceBuilder.MapEnum<WorkplaceType>();
        dataSourceBuilder.MapEnum<ExperienceLevel>();
        dataSourceBuilder.MapEnum<OfferStatus>();
        var dataSource = dataSourceBuilder.Build();

        // Register the data source as a singleton
        services.AddSingleton(dataSource);

        services.AddDbContext<ReadApiDbContext>((serviceProvider, options) =>
        {
            var ds = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            options.UseNpgsql(ds)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IJobAdReadRepository, JobAdReadRepository>();

        return services;
    }
}

