using Microsoft.Data.Sqlite;

var dbDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var dbPath = Path.Combine(dbDir, "school_events.db");
var schemaPath = Path.Combine(dbDir, "schema.sql");
var seedPath = Path.Combine(dbDir, "seed.sql");

if (!File.Exists(schemaPath))
    throw new FileNotFoundException("schema.sql not found", schemaPath);

if (File.Exists(dbPath))
    File.Delete(dbPath);

await using var connection = new SqliteConnection($"Data Source={dbPath}");
await connection.OpenAsync();

await ExecuteScriptAsync(connection, await File.ReadAllTextAsync(schemaPath));

if (File.Exists(seedPath))
    await ExecuteScriptAsync(connection, await File.ReadAllTextAsync(seedPath));

await using var cmd = connection.CreateCommand();
cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
await using var reader = await cmd.ExecuteReaderAsync();
var tables = new List<string>();
while (await reader.ReadAsync())
    tables.Add(reader.GetString(0));

Console.WriteLine($"Created {dbPath}");
Console.WriteLine($"Tables: {string.Join(", ", tables)}");

static async Task ExecuteScriptAsync(SqliteConnection connection, string script)
{
    await using var cmd = connection.CreateCommand();
    cmd.CommandText = script;
    await cmd.ExecuteNonQueryAsync();
}
