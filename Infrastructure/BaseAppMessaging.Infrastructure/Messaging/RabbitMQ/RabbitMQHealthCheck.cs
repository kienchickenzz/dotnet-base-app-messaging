/**
 * RabbitMQHealthCheck verifies RabbitMQ broker connectivity.
 *
 * <p>Uses singleton IConnection to check broker availability
 * without creating new connections on each health check call.</p>
 */

namespace BaseAppMessaging.Infrastructure.Messaging.RabbitMQClient;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;


/// <summary>
/// Health check for RabbitMQ connection status.
/// </summary>
/// <remarks>
/// Reuses the singleton IConnection registered in DI
/// to avoid expensive connection creation on each check.
/// </remarks>
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly IConnection _connection;

    /// <summary>
    /// Initializes health check with existing RabbitMQ connection.
    /// </summary>
    /// <param name="connection">Singleton RabbitMQ connection from DI.</param>
    public RabbitMQHealthCheck(IConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if connection is still open
            if (_connection.IsOpen)
            {
                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Connected to {_connection.Endpoint.HostName}"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy(
                "RabbitMQ connection is closed"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(
                context.Registration.FailureStatus,
                "RabbitMQ health check failed",
                ex));
        }
    }
}
