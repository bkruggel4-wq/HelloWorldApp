
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Read connection string from environment
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new Exception("DATABASE_URL is missing");

// Convert Render URL into a PostgreSQL connection string
var uri = new Uri(connectionString);

var userInfo = uri.UserInfo.Split(':');

var csb = new NpgsqlConnectionStringBuilder
{
    Host = uri.Host,
    Port = uri.IsDefaultPort || uri.Port == -1
        ? 5432
        : uri.Port,
    Database = uri.AbsolutePath.TrimStart('/'),
    Username = Uri.UnescapeDataString(userInfo[0]),
    Password = Uri.UnescapeDataString(userInfo[1]),
    SslMode = SslMode.Require
};

// HTTP endpoint: Database connection test
app.MapGet("/", async () =>
{
    await using var connection =
        new NpgsqlConnection(csb.ConnectionString);

    await connection.OpenAsync();

    await using var command =
        new NpgsqlCommand("SELECT NOW()", connection);

    var result = await command.ExecuteScalarAsync();

    return $"Database connection successful! Server time: {result}";
});

// HTTP endpoint: Query all users
app.MapGet("/users", async () =>
{
    var users = new List<object>();

    await using var connection =
        new NpgsqlConnection(csb.ConnectionString);

    await connection.OpenAsync();

    await using var command =
        new NpgsqlCommand(
            "SELECT id, name, email FROM users",
            connection);

    await using var reader =
        await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        users.Add(new
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2)
        });
    }

    return Results.Ok(users);
});

app.Run();




