namespace WebApi.Helpers;

public static class DatabaseConfigHelper
{
    public static string GetPostgresConnectionString(IConfiguration configuration)
    {
        var dbConfig = configuration.GetSection("Database:PostgreSQL");

        var server = dbConfig["Server"];
        var userId = dbConfig["UserId"];
        var password = dbConfig["Password"];
        var database = dbConfig["Database"];

        return $"Host={server};Database={database};Username={userId};Password={password};Include Error Detail=true;";
    }
}
