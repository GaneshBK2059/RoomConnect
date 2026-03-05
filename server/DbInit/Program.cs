using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing RoomConnect Database...");

        string baseConnString = "Host=localhost;Port=5432;Username=postgres;Password=G@nesh444;Database=postgres";
        string targetDb = "roomconnect";
        string targetConnString = $"Host=localhost;Port=5432;Username=postgres;Password=G@nesh444;Database={targetDb}";
        
        try
        {
            // 1. Create DB if not exists
            await using (var conn = new NpgsqlConnection(baseConnString))
            {
                await conn.OpenAsync();
                
                bool dbExists = false;
                await using (var checkCmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{targetDb}'", conn))
                {
                    var result = await checkCmd.ExecuteScalarAsync();
                    dbExists = result != null;
                }

                if (!dbExists)
                {
                    Console.WriteLine($"Creating database {targetDb}...");
                    await using (var createCmd = new NpgsqlCommand($"CREATE DATABASE {targetDb}", conn))
                    {
                        await createCmd.ExecuteNonQueryAsync();
                    }
                    Console.WriteLine("Database created.");
                }
                else
                {
                    Console.WriteLine($"Database {targetDb} already exists.");
                }
            }

            // 2. Run the SQL script
            string scriptPath = Path.Combine("..", "SqlScripts", "01_init_database.sql");
            string sqlScript = await File.ReadAllTextAsync(scriptPath);
            
            sqlScript = "DROP SCHEMA public CASCADE; CREATE SCHEMA public; " + sqlScript;

            Console.WriteLine("Executing schema initialization script...");
            await using (var conn = new NpgsqlConnection(targetConnString))
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(sqlScript, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            
            Console.WriteLine("Database initialization complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
