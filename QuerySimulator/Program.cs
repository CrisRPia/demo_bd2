// See https://aka.ms/new-console-template for more information

// See https://aka.ms/new-console-template for more information
using Npgsql;
namespace QuerySimulator;

public class Program
{
    private static readonly Random Rng = new();
    private static readonly string[] Names = ["Alice", "Bob", "Charlie", "Diana", "Eve", "Frank", "Grace", "Heidi"];
    private static readonly string[] ActionDescriptors = ["login", "logout", "view_page", "update_profile", "post_comment", "delete_post"];

    public static async Task Main()
    {
        Console.WriteLine("C# Query Simulator starting...");

        // Get connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Error: DB_CONNECTION_STRING environment variable not set.");
            return;
        }

        // Wait a moment for the database to be ready
        Console.WriteLine("Waiting for the database to be ready...");
        await Task.Delay(5000);

        await using var dataSource = NpgsqlDataSource.Create(connectionString);

        Console.WriteLine("Starting database query simulation loop.");
        while (true)
        {
            try
            {
                // 1. Add a new user and a few actions for them
                await AddUserAndActions(dataSource);

                // 2. Run the more complex SELECT query
                await GetRecentUserActions(dataSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in the main loop: {ex.Message}");
            }

            // 3. Wait a random amount of time before the next cycle
            var waitTime = Rng.Next(3000, 10000); // Wait between 3 and 10 seconds
            Console.WriteLine($"\n...Waiting for {waitTime / 1000.0} seconds before next cycle...\n");
            await Task.Delay(waitTime);
        }
    }

    private static async Task AddUserAndActions(NpgsqlDataSource dataSource)
    {
        // Add a new user
        var randomName = Names[Rng.Next(Names.Length)];
        var birthDate = new DateOnly(Rng.Next(1980, 2006), Rng.Next(1, 13), Rng.Next(1, 29));

        long newUserId;
        using var cmd = dataSource.CreateCommand("INSERT INTO users (name, birth) VALUES ($1, $2) RETURNING id");
        cmd.Parameters.AddWithValue(randomName);
        cmd.Parameters.AddWithValue(birthDate);
        newUserId = (int)(await cmd.ExecuteScalarAsync() ?? -1);

        if (newUserId != -1)
        {
            Console.WriteLine($"- User '{randomName}' added with ID: {newUserId}");

            // Add a random number of actions for the new user
            var actionCount = Rng.Next(1, 6);
            for (int i = 0; i < actionCount; i++)
            {
                var randomAction = ActionDescriptors[Rng.Next(ActionDescriptors.Length)];
                using var actionCmd = dataSource.CreateCommand("INSERT INTO actions (descriptor, userid, done_at) VALUES ($1, $2, NOW())");
                actionCmd.Parameters.AddWithValue(randomAction);
                actionCmd.Parameters.AddWithValue(newUserId);
                await actionCmd.ExecuteNonQueryAsync();
                Console.WriteLine($"-- Action '{randomAction}' recorded for user ID: {newUserId}");
                await Task.Delay(Rng.Next(200, 800));
            }
        }
    }

    private static async Task GetRecentUserActions(NpgsqlDataSource dataSource)
    {
        Console.WriteLine("-> Running complex query: Fetching recent user actions...");
        var query = @"
            SELECT
                u.name,
                u.birth,
                a.descriptor,
                a.done_at
            FROM users u
            JOIN actions a ON u.id = a.userid
            WHERE a.done_at > NOW() - INTERVAL '5 minutes'
            ORDER BY a.done_at DESC
            LIMIT 10;";

        await using var cmd = dataSource.CreateCommand(query);
        await using var reader = await cmd.ExecuteReaderAsync();

        int rowCount = 0;
        while (await reader.ReadAsync())
        {
            rowCount++;
        }
        Console.WriteLine($"<- Query complete. Found {rowCount} recent actions.");
    }
}

