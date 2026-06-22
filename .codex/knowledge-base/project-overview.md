# Project Overview

## Current State

School Events Notification Center is a C#/.NET solution scaffold for a school
events app. The repository is mostly structure: layers, project references,
static UI shell, database DDL, and worker skeleton are present, while feature
implementation is still TODO.

The solution file is `SchoolEvents.slnx` and includes four projects:

- `PresentationLayer1` - ASP.NET Core host serving static UI and API endpoints.
- `BusinessLogicLayer2` - service/rules layer and dependency wiring into Data.
- `DataAccessLayer3` - EF Core/data layer and SQLite schema assets.
- `Worker` - background worker host for notification processing.

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

Both hosts load `.env` with DotNetEnv and require `DATABASE_URL`.

Local database notes in `DataAccessLayer3/Db/README.md` describe SQLite usage:

```text
DATABASE_URL=Data Source=DataAccessLayer3/Db/school_events.db
```

The worker also reads `WORKER_POLL_SECONDS`; it defaults to 5 seconds.

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
- Presentation: ASP.NET Core, controllers, JWT bearer auth package, DotNetEnv
- Business: BCrypt.Net-Next, MailKit, DI abstractions
- Data: EF Core 9, EF Core SQLite, EFCore.NamingConventions
- Worker: .NET Worker Service, DotNetEnv

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

## Static UI

`PresentationLayer1/wwwroot` contains a minimal shell:

- `index.html` with nav links for Events, My signups, My badges, and Login.
- `app.js` currently contains only a skeleton comment.
- `style.css` provides simple responsive styling.

## Likely Next Implementation Path

1. Add EF entities under `DataAccessLayer3/Models`.
2. Add `AppDbContext` under `DataAccessLayer3/Db`.
3. Register SQLite DbContext and repositories in `AddDataLayer`.
4. Add repository interfaces/implementations under `DataAccessLayer3/Repositories`.
5. Add business DTOs/services under `BusinessLogicLayer2`.
6. Register business services in `AddBusinessLayer`.
7. Add thin controllers under `PresentationLayer1/Controllers`.
8. Implement notification job claiming/sending in Worker through Business services.

## Notes From Exploration

- Some README/schema text appears mojibake-encoded in the terminal output, but
  the intent is still readable.
- `git` was not available in the current shell, so status could not be checked.
- No existing `.codex` or `.agents` folder was present before this knowledge
  base was initialized.

