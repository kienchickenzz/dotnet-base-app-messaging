/**
 * Dependency injection configuration for Infrastructure layer.
 *
 * <p>Registers Hangfire background jobs, messaging, and related services.</p>
 */

namespace BaseAppMessaging.Infrastructure;

using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using BaseAppMessaging.Application.Common.ApplicationServices.BackgroundJob;
using BaseAppMessaging.Application.Common.ApplicationServices.Messaging;
using BaseAppMessaging.Infrastructure.Settings;
using BaseAppMessaging.Infrastructure.BackgroundJobs;
using BaseAppMessaging.Infrastructure.Messaging.Fake;


public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure services for API/Producer (includes Hangfire).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            ._AddSettings(config)
            ._AddBackgroundJobs(config)
            ._AddMessaging(config)
            ._AddServices();

        return services;
    }

    /// <summary>
    /// Registers Infrastructure services for Worker/Consumer (no Hangfire, no DB).
    /// </summary>
    public static IServiceCollection AddInfrastructureForWorker(this IServiceCollection services, IConfiguration config)
    {
        services
            ._AddSettings(config)
            ._AddMessaging(config);

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration configuration)
    {
        builder
            ._UseHangfireDashboard();

        return builder;
    }

    /// <summary>
    /// Registers configuration settings with IOptions pattern.
    /// </summary>
    private static IServiceCollection _AddSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<HangfireSettings>(config.GetSection(HangfireSettings.SectionName));
        services.Configure<MailSettings>(config.GetSection(MailSettings.SectionName));
        services.Configure<MessagingSettings>(config.GetSection(MessagingSettings.SectionName));

        return services;
    }

    private static IServiceCollection _AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJobService, HangfireService>();

        return services;
    }

    internal static IServiceCollection _AddBackgroundJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfireServer();

        services.AddHangfire(hangfireConfig => hangfireConfig
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection")) // Lưu jobs vào SQL Server
            .UseFilter(new LogJobFilter())); // Gắn filter để log job lifecycle

        return services;
    }

    /// <summary>
    /// Configures Hangfire dashboard.
    /// </summary>
    private static IApplicationBuilder _UseHangfireDashboard(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices
            .GetRequiredService<IOptionsMonitor<HangfireSettings>>()
            .CurrentValue;

        var dashboardOptions = new DashboardOptions
        {
            AppPath = settings.Dashboard.AppPath,
            StatsPollingInterval = settings.Dashboard.StatsPollingInterval,
            DashboardTitle = settings.Dashboard.DashboardTitle,
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = settings.Credentials.User,
                    Pass = settings.Credentials.Password
                }
            }
        };

        return app.UseHangfireDashboard(settings.Route, dashboardOptions);
    }

    /// <summary>
    /// Registers messaging services based on configured provider.
    /// </summary>
    private static IServiceCollection _AddMessaging(this IServiceCollection services, IConfiguration config)
    {
        var settings = config
            .GetSection(MessagingSettings.SectionName)
            .Get<MessagingSettings>();

        // Register provider based on configuration
        switch (settings?.Provider)
        {
            case MessagingProviderEnum.RabbitMQ:
                services._AddRabbitMQMessaging(settings.RabbitMQ);
                break;
            case MessagingProviderEnum.Kafka:
                services._AddKafkaMessaging(settings.Kafka);
                break;
            case MessagingProviderEnum.Fake:
            default:
                services._AddFakeMessaging();
                break;
        }

        return services;
    }

    /// <summary>
    /// Registers Fake messaging provider (for development/testing).
    /// </summary>
    private static IServiceCollection _AddFakeMessaging(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IMessageSender<>), typeof(FakeSender<>));
        services.AddSingleton(typeof(IMessageReceiver<,>), typeof(FakeReceiver<,>));

        return services;
    }

    /// <summary>
    /// Registers RabbitMQ messaging provider.
    /// </summary>
    private static IServiceCollection _AddRabbitMQMessaging(this IServiceCollection services, RabbitMQSettings? settings)
    {
        // TODO: Implement RabbitMQ provider
        // services.AddSingleton(typeof(IMessageSender<>), typeof(RabbitMQSender<>));
        // services.AddSingleton(typeof(IMessageReceiver<,>), typeof(RabbitMQReceiver<,>));

        throw new NotImplementedException("RabbitMQ provider not yet implemented. Use 'Fake' provider for development.");
    }

    /// <summary>
    /// Registers Kafka messaging provider.
    /// </summary>
    private static IServiceCollection _AddKafkaMessaging(this IServiceCollection services, KafkaSettings? settings)
    {
        // TODO: Implement Kafka provider
        // services.AddSingleton(typeof(IMessageSender<>), typeof(KafkaSender<>));
        // services.AddSingleton(typeof(IMessageReceiver<,>), typeof(KafkaReceiver<,>));

        throw new NotImplementedException("Kafka provider not yet implemented. Use 'Fake' provider for development.");
    }
}
