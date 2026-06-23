# Project Overview

## Current State

School Events Notification Center is a C#/.NET solution. The Razor Pages frontend
and MockServer are fully operational. The business and data layers and the Worker
remain TODO structure.

The solution file is `SchoolEvents.slnx` and includes four projects (plus a temporary fifth):

- `PresentationLayer1` - ASP.NET Core Razor Pages frontend (port 5080).
- `BusinessLogicLayer2` - service/rules layer and dependency wiring into Data (TODO).
- `DataAccessLayer3` - EF Core/data layer and SQLite schema assets (TODO).
- `Worker` - background worker host for notification processing (skeleton only).
- `MockServer` - temporary minimal API mock backend (port 5090); implements the full REST
  surface backed by SQLite seeded from `schema.sql`. Remove once real backend is implemented.

## Architecture Rules

Dependency direction is:

```text
PresentationLayer1 -> BusinessLogicLayer2 -> DataAccessLayer3
Worker             -> BusinessLogicLayer2 -> DataAccessLayer3
```

Hosts should call `AddBusinessLayer(databaseUrl)`. The Business layer calls
`AddDataLayer(databaseUrl)`, so Presentation and Worker should not reference
`DataAccessLayer3` directly.

## Runtime Configuration

Both hosts load `.env` with DotNetEnv when present; `appsettings.Development.json` provides
fallback defaults for the mock stack so no `.env` is required to run it.

| File | Use |
|---|---|
| `appsettings.Development.json` | `DATABASE_URL` (SQLite) and `MOCK_LOGIN=false`; mock stack needs no extra setup |
| `.env.mockdev.example` | Copy to `.env` to enable mock login dropdown (`MOCK_LOGIN=true`) |
| `.env.dev.example` | Copy to `.env` for real backend local dev (SQLite, MailHog, dev JWT secret) |

The worker also reads `WORKER_POLL_SECONDS` (default 5 s) and `WORKER_MAX_ATTEMPTS` (default 3).

## Data Model Plan

`DataAccessLayer3/Db/schema.sql` defines the intended SQLite tables:

- `users`
- `events`
- `registrations`
- `notification_jobs`
- `notification_log`
- `badges`
- `attendances`
- `user_badges`

Important constraints include role/status checks, one active registration per
user/event, FIFO waitlist indexing, notification job idempotency, and one
successful notification log per job.

## Packages And Stack

- Target framework: `net10.0`
- **PresentationLayer1**: ASP.NET Core Razor Pages, session auth, typed HttpClient
  (`ApiClient`), JWT bearer auth package, DotNetEnv
- **MockServer**: ASP.NET Core Minimal API, Microsoft.Data.Sqlite, single-file architecture
- **BusinessLogicLayer2**: BCrypt.Net-Next, MailKit, DI abstractions
- **DataAccessLayer3**: EF Core 9, EF Core SQLite, EFCore.NamingConventions
- **Worker**: .NET Worker Service, DotNetEnv

The README mentions PostgreSQL/Npgsql as a future or production target, but the
checked-in project currently references SQLite and comments out Npgsql.

## Existing Entry Points

- `PresentationLayer1/Program.cs`
  - loads `.env`
  - requires `DATABASE_URL`
  - wires `AddBusinessLayer(databaseUrl)`
  - serves `wwwroot`
  - maps controllers
  - exposes `GET /health` returning `{ status = "ok" }`

- `Worker/Program.cs`
  - loads `.env`
  - requires `DATABASE_URL`
  - wires `AddBusinessLayer(databaseUrl)`
  - registers `NotificationWorker`

- `Worker/NotificationWorker.cs`
  - background loop skeleton
  - logs startup
  - delays by configured poll interval
  - TODO: claim and process notification jobs

## Frontend Structure

`PresentationLayer1` is a full Razor Pages frontend:

```
Pages/
  Events/Index.cshtml        student event catalog
  Events/Details.cshtml      event detail + register/cancel
  Registrations/Me.cshtml    student's own registrations
  Organizer/Events/          organizer CRUD, preview, registrations list
  Login.cshtml               real login form (default) or mock dropdown (MOCK_LOGIN=true)
Services/
  IApiClient / ApiClient       typed HttpClient calling the backend API (mock or real)
  IAuthSession / AuthSession   session-based user identity (id/email/role/name)
wwwroot/
  app.js      local-timezone time display, copy-link clipboard buttons
  style.css   responsive styles
```

## Likely Next Implementation Path

1. Add EF entities under `DataAccessLayer3/Models`.
2. Add `AppDbContext` under `DataAccessLayer3/Db`.
3. Register SQLite DbContext and repositories in `AddDataLayer`.
4. Add repository interfaces/implementations under `DataAccessLayer3/Repositories`.
5. Add business DTOs/services under `BusinessLogicLayer2`.
6. Register business services in `AddBusinessLayer`.
7. Add thin controllers under `PresentationLayer1/Controllers`.
8. Implement notification job claiming/sending in Worker through Business services.

## Notes

- The `DataAccessLayer3/DataAccessLayer3.csproj` excludes `Db/InitDb/**` via
  `<Compile Remove>` to prevent the nested InitDb `Program.cs` from conflicting
  with the parent project's compilation.
- DotNetEnv 3.x removed `Env.TraverseUp()`. Both hosts use a one-level traversal:
  load `.env` from CWD if present, otherwise try `../.env` — so `dotnet run --project`
  invocations from the project directory still find the repo-root `.env`.
- Config is read via `builder.Configuration["KEY"]`, not `Environment.GetEnvironmentVariable`,
  so `appsettings.*.json` values take effect even without a `.env` file.

