using BusinessLogicLayer2;
using DotNetEnv;
using Worker;

// Load .env from CWD; fall back one level up for `dotnet run --project` invocations
var envPath = File.Exists(".env") ? ".env" : Path.Combine("..", ".env");
Env.Load(envPath);

var builder = Host.CreateApplicationBuilder(args);

var databaseUrl = builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("Missing required config: DATABASE_URL");

// Same layers as the API host — Business pulls in Data.
builder.Services.AddBusinessLayer(databaseUrl);
builder.Services.AddHostedService<NotificationWorker>();

builder.Build().Run();
