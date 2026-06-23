using Microsoft.Data.Sqlite;

var dbDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var dbPath = Path.Combine(dbDir, "school_events.db");
var migrationsDir = Path.Combine(dbDir, "migrations");
var seedPath = Path.Combine(dbDir, "seed.sql");

if (!Directory.Exists(migrationsDir))
    throw new DirectoryNotFoundException($"migrations/ not found: {migrationsDir}");

if (File.Exists(dbPath))
    File.Delete(dbPath);

await using var connection = new SqliteConnection($"Data Source={dbPath}");
await connection.OpenAsync();

await ExecuteAsync(connection, "PRAGMA foreign_keys = ON;");
await ExecuteAsync(connection, "PRAGMA journal_mode = WAL;");

await ExecuteAsync(connection, """
    CREATE TABLE schema_migrations (
        id          INTEGER PRIMARY KEY AUTOINCREMENT,
        name        TEXT NOT NULL UNIQUE,
        applied_at  TEXT NOT NULL DEFAULT (datetime('now'))
    );
    """);

var migrationFiles = Directory.GetFiles(migrationsDir, "*.sql").OrderBy(f => f).ToList();
if (migrationFiles.Count == 0)
    throw new InvalidOperationException("No migration files found in migrations/");

foreach (var file in migrationFiles)
{
    var name = Path.GetFileName(file);
    await ExecuteAsync(connection, await File.ReadAllTextAsync(file));

    await using var insert = connection.CreateCommand();
    insert.CommandText = "INSERT INTO schema_migrations (name) VALUES ($name)";
    insert.Parameters.AddWithValue("$name", name);
    await insert.ExecuteNonQueryAsync();

    Console.WriteLine($"Applied migration: {name}");
}

if (File.Exists(seedPath))
    await ExecuteAsync(connection, await File.ReadAllTextAsync(seedPath));

await using var cmd = connection.CreateCommand();
cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
await using var reader = await cmd.ExecuteReaderAsync();
var tables = new List<string>();
while (await reader.ReadAsync())
    tables.Add(reader.GetString(0));

Console.WriteLine($"Created {dbPath}");
Console.WriteLine($"Tables: {string.Join(", ", tables)}");

static async Task ExecuteAsync(SqliteConnection connection, string script)
{
    await using var cmd = connection.CreateCommand();
    cmd.CommandText = script;
    await cmd.ExecuteNonQueryAsync();
}
