namespace Prictionary.Database;
public static class ConnectionStringBuilder
{
    public static string BuildPostgres()
    {
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        
        return $"postgresql://{dbUser}:{dbPassword}@{dbHost}:{dbPort}/{dbName}";
    }
}