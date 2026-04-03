/**
 * HealthCheckExtensions provides custom response writer for health checks.
 *
 * <p>Returns detailed JSON response with status of each health check component.</p>
 */

namespace BaseAppMessaging.Api.Extensions;

using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;


/// <summary>
/// Extension methods for health check configuration.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Maps health check endpoint with detailed JSON response.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <param name="path">The endpoint path (default: /health).</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapHealthChecksWithJsonResponse(
        this WebApplication app,
        string path = "/health")
    {
        app.MapHealthChecks(path, new HealthCheckOptions
        {
            ResponseWriter = _WriteJsonResponse
        });

        return app;
    }

    /// <summary>
    /// Writes health check result as JSON response.
    /// </summary>
    private static async Task _WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString(),
                tags = entry.Value.Tags,
                error = entry.Value.Exception?.Message
            })
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
