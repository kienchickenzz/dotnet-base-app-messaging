/**
 * ApplicationDbContextHealthCheck verifies database connectivity.
 *
 * <p>Executes a simple query to ensure the database
 * is accessible and responding.</p>
 */

namespace BaseAppMessaging.Persistence.DatabaseContext;

using Microsoft.Extensions.Diagnostics.HealthChecks;


/// <summary>
/// Health check for database connectivity via ApplicationDbContext.
/// </summary>
/// <remarks>
/// Uses EF Core's CanConnectAsync to verify database availability
/// without executing heavy queries.
/// </remarks>
public class ApplicationDbContextHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes health check with database context.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public ApplicationDbContextHealthCheck(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // CanConnectAsync executes a simple connectivity check
            if (await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                return HealthCheckResult.Healthy("Database connection is healthy");
            }

            return HealthCheckResult.Unhealthy("Cannot connect to database");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "Database health check failed",
                ex);
        }
    }
}
