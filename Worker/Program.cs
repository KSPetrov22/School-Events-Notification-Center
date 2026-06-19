using BusinessLogicLayer2;
using DotNetEnv;
using Worker;

// Load .env (see .env.example) into environment variables for local dev.
Env.TraverseUp().Load();

var builder = Host.CreateApplicationBuilder(args);

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Missing required env var: DATABASE_URL");

// Same layers as the API host — Business pulls in Data.
builder.Services.AddBusinessLayer(databaseUrl);
builder.Services.AddHostedService<NotificationWorker>();

builder.Build().Run();
