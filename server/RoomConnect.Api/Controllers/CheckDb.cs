using Microsoft.AspNetCore.Mvc;
using RoomConnect.Infrastructure.Database;
using Npgsql;

namespace RoomConnect.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IDbConnectionFactory _dbFactory;

        public HealthController(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet("db")]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                await using var connection = (NpgsqlConnection)_dbFactory.CreateConnection();
                await connection.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT NOW()", connection);
                var result = await cmd.ExecuteScalarAsync();

                return Ok(new
                {
                    status = "connected",
                    time = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        [HttpPost("migrate")]
        public async Task<IActionResult> RunMigration()
        {
            try
            {
                var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SqlScripts", "02_LandlordModule_Updates.sql");
                if (!System.IO.File.Exists(sqlPath))
                {
                    return NotFound(new { message = $"Migration file not found at {sqlPath}" });
                }

                var sql = await System.IO.File.ReadAllTextAsync(sqlPath);

                await using var connection = (NpgsqlConnection)_dbFactory.CreateConnection();
                await connection.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, connection);
                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Migration executed successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Migration failed", error = ex.Message });
            }
        }

        [HttpPost("reset-db")]
        public async Task<IActionResult> ResetDatabase()
        {
            try
            {
                var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SqlScripts", "01_init_database.sql");
                if (!System.IO.File.Exists(sqlPath))
                {
                    return NotFound(new { message = $"Init file not found at {sqlPath}" });
                }

                var cleanSql = "DROP SCHEMA public CASCADE; CREATE SCHEMA public;";
                var initSql = await System.IO.File.ReadAllTextAsync(sqlPath);

                await using var connection = (NpgsqlConnection)_dbFactory.CreateConnection();
                await connection.OpenAsync();

                await using var cleanCmd = new NpgsqlCommand(cleanSql, connection);
                await cleanCmd.ExecuteNonQueryAsync();

                await using var initCmd = new NpgsqlCommand(initSql, connection);
                await initCmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Database successfully reset to the latest schema! Dummy data for users created. Rooms/Bookings are cleanly wiped." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Reset failed", error = ex.Message });
            }
        }
    }
}
