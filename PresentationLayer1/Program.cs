using BusinessLogicLayer2;
using DotNetEnv;

// Entry point (= the reference's main/app.py): load config, wire the layers,
// serve the static UI, expose the API. Skeleton only — features go in per epic.

// Load .env (see .env.example) into environment variables for local dev.
Env.TraverseUp().Load();

var builder = WebApplication.CreateBuilder(args);

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Missing required env var: DATABASE_URL");

// Wire the layers (Presentation -> Business -> Data).
builder.Services.AddBusinessLayer(databaseUrl);
builder.Services.AddControllers();

var app = builder.Build();

// Serve the static web UI from wwwroot/ (the reference's static/).
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// TODO: JWT auth, CORS, authorization policies (Epic 1).

app.Run();
