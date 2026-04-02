namespace BaseAppMessaging.Application.Common.ApplicationServices.DataAccess;

using System.Data;


/// <summary>
/// Factory for creating database connections.
/// </summary>
/// <remarks>
/// Provides raw SQL connection for scenarios requiring direct database access,
/// such as Dapper queries or complex reporting.
/// </remarks>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    /// <returns>An open database connection.</returns>
    IDbConnection CreateConnection();
}
