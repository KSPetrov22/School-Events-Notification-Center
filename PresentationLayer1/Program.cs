using BusinessLogicLayer2;
using DotNetEnv;
using PresentationLayer1.Services;

// Entry point (= the reference's main/app.py): load config, wire the layers,
// serve the static UI, expose the API. Skeleton only — features go in per epic.

// Load .env (see .env.example) into environment variables for local dev.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

var databaseUrl = builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("Missing required config: DATABASE_URL");

// Wire the layers (Presentation -> Business -> Data).
builder.Services.AddBusinessLayer(databaseUrl);
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var mockApiBaseUrl = builder.Configuration["MockApiBaseUrl"] ?? "http://localhost:5090";
builder.Services.AddHttpClient<IMockApiClient, MockApiClient>(client =>
{
    client.BaseAddress = new Uri(mockApiBaseUrl);
});
builder.Services.AddScoped<IAuthSession, AuthSession>();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();

app.MapControllers();
app.MapRazorPages();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// TODO: JWT auth, CORS, authorization policies (Epic 1).

app.Run();
