using BusinessLogicLayer2;
using DotNetEnv;
using Worker;

// Load .env (see .env.example) into environment variables for local dev.
Env.Load();

var builder = Host.CreateApplicationBuilder(args);

var databaseUrl = builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("Missing required config: DATABASE_URL");

// Same layers as the API host — Business pulls in Data.
builder.Services.AddBusinessLayer(databaseUrl);
builder.Services.AddHostedService<NotificationWorker>();

builder.Build().Run();
