/**
 * Dependency injection configuration for Infrastructure layer.
 *
 * <p>Registers Hangfire background jobs and related services.</p>
 */

namespace BaseAppMessaging.Infrastructure;

using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using BaseAppMessaging.Application.Common.ApplicationServices.BackgroundJob;
using BaseAppMessaging.Infrastructure.Settings;
using BaseAppMessaging.Infrastructure.BackgroundJobs;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            ._AddSettings(config)
            ._AddBackgroundJobs(config)
            ._AddServices();

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
}
