namespace BaseAppMessaging.Persistence;

using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using BaseAppMessaging.Persistence.Settings;
using BaseAppMessaging.Application.Common.ApplicationServices.DataAccess;
using BaseAppMessaging.Application.Common.ApplicationServices.Repositories;
using BaseAppMessaging.Application.Common.ApplicationServices.BackgroundJob;
using BaseAppMessaging.Persistence.Common;
using BaseAppMessaging.Persistence.Repositories;
using BaseAppMessaging.Persistence.DatabaseContext;
using BaseAppMessaging.Persistence.BackgroundJobs.Outbox;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                  throw new ArgumentNullException(nameof(configuration));


        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    private static IServiceCollection _AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxSettings>(configuration.GetSection("OutboxSettings"));
        return services;
    }

    /// <summary>
    /// Registers recurring job to process outbox messages.
    /// </summary>
    public static void AddOutBoxJob(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var job = scope.ServiceProvider.GetRequiredService<IJobService>();
        var settings = scope.ServiceProvider
            .GetRequiredService<IOptions<OutboxSettings>>()
            .Value;

        job.Recurring<ProcessOutboxMessagesJob>(
            "ProcessOutboxMessages",
            job => job.Execute(),
            $"*/{settings.IntervalInMinutes} * * * *");
    }

    /// <summary>
    /// Adds Persistence health checks (database connectivity).
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <returns>The health checks builder for chaining.</returns>
    public static IHealthChecksBuilder AddPersistenceHealthChecks(this IHealthChecksBuilder builder)
    {
        builder.AddCheck<ApplicationDbContextHealthCheck>(
            name: "database",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "db", "sql" });

        return builder;
    }
}
