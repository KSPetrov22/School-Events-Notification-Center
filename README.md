# School Events — Notification Center

**Structure only** — the empty skeleton of a 3-layer architecture in C# / .NET,
organized after the layered convention in
`on-the-job-training-11b-SKDimitrov22` (`PresentationLayer1` /
`BusinessLogicLayer2` / `DataAccessLayer3`). No features are implemented yet:
the projects, folders, package references and layer wiring are in place; the
code that fills them is `TODO` per epic.

> The reference project is Python/FastAPI; only its **architecture** is
> mirrored here. The stack is C#/.NET + EF Core.

## The three layers

```
PresentationLayer1  ──►  BusinessLogicLayer2  ──►  DataAccessLayer3
   (HTTP / UI)              (services, rules)        (EF Core + Postgres)
```

Dependencies point inward only. Both hosts (`PresentationLayer1`, `Worker`) call
`AddBusinessLayer()`, which pulls in the Data layer — neither references
`DataAccessLayer3` directly.

```
BusinessLogicLayer2/       the rules (= reference services/)
  Services/                (empty) service interfaces + implementations
  Dtos/                    (empty) what Presentation sees (never entities)
  DependencyInjection.cs   AddBusinessLayer() — registers services, calls AddDataLayer()
DataAccessLayer3/          EF Core (= reference models.py + db.py + repositories/)
  Models/                  (empty) entities = the tables
  Db/                      (empty) AppDbContext + connection helper
  Repositories/            (empty) data-access classes
  Configurations/          (empty) Fluent API, one per entity
  DependencyInjection.cs   AddDataLayer() — registers DbContext + repositories
Worker/                    background host on the same layers (references Business)
  Program.cs  NotificationWorker.cs   poll-loop skeleton (Epic 4)
SchoolEvents.slnx          the four projects
```

(Empty folders are kept in git with a `.gitkeep` placeholder; delete it once you
add real files.)

**Frontend — PresentationLayer1** (Razor Pages host, port 5080)

```
PresentationLayer1/
  Program.cs               entry point: config, DI, session, HttpClient for mock API
  Pages/                   Razor Pages UI (Events, Organizer/Events, Registrations, Login)
  Services/
    IApiClient.cs          typed HttpClient interface — swap base URL to point at real backend
    ApiClient.cs           calls the backend API over HTTP; sends Authorization: Bearer
    IAuthSession.cs        session abstraction
    AuthSession.cs         stores user id/email/role/name in ASP.NET Core session
  wwwroot/                 static assets (app.js, style.css)
```

**MockServer** (temporary mock API, port 5090)

```
MockServer/
  Program.cs               single-file minimal API — all endpoints + SQLite seed in one file
  App_Data/                auto-created SQLite database (gitignored)
```

Reads `DataAccessLayer3/Db/schema.sql` on first run, seeds 4 users and 3 events.
Issues `alg:none` JWT tokens at `/login` (password is accepted but ignored).
Reads user identity from `Authorization: Bearer` on all other requests.

## Where to start filling it in

1. `DataAccessLayer3/Models/` — add entity classes (one per table).
2. `DataAccessLayer3/Db/AppDbContext.cs` — add a `DbContext` with `DbSet`s, then
   register it inside `AddDataLayer`.
3. `DataAccessLayer3/Repositories/` — one repository (interface + impl) per area.
4. `BusinessLogicLayer2/Services/` — services that use the repositories; register
   them inside `AddBusinessLayer`.
5. `PresentationLayer1/Controllers/` — thin controllers that call the services.

## Tech stack (packages already referenced)

- .NET 10 · ASP.NET Core (controllers) · .NET Worker Service
- EF Core + Npgsql (PostgreSQL) · EFCore.NamingConventions (snake_case columns)
- JWT Bearer auth · BCrypt password hashing · DotNetEnv

**Frontend (PresentationLayer1)**
- ASP.NET Core Razor Pages · session-based auth · typed HttpClient (`ApiClient`)
- DotNetEnv · plain HTML/CSS/JS in `wwwroot/`

**MockServer**
- ASP.NET Core Minimal API · Microsoft.Data.Sqlite · single-file architecture

## Run

### Mock stack — frontend + mock login (works today)

```bash
cp .env.mockdev.example .env   # enables mock login dropdown

# Terminal 1 — mock API (auto-creates SQLite DB and seeds data on first run)
dotnet run --project MockServer           # → http://localhost:5090

# Terminal 2 — Razor Pages UI
dotnet run --project PresentationLayer1   # → http://localhost:5080
```

Open **http://localhost:5080**. The login page shows a dropdown of seeded accounts — no password needed.

### Docker mock stack

```bash
docker compose up --build
```

Open **http://localhost:5080**. Compose starts:

- `presentation` on `http://localhost:5080`
- `mockserver` on `http://localhost:5090`

The mock API stores its SQLite database in the `mockserver-data` Docker volume.
Use `docker compose down -v` when you want to reset the seeded mock data.

### Docker real backend stack

```bash
docker compose -f compose.real.yaml up --build
```

Open **http://localhost:5080**. This uses the real `/api` controllers backed by
EF Core + SQLite instead of `MockServer`. Seeded login credentials:

- `student1@school.local` / `password`
- `student2@school.local` / `password`
- `organizer1@school.local` / `password`

The real backend stores SQLite data in the `presentation-data` Docker volume.
Use this to reset it:

```bash
docker compose -f compose.real.yaml down -v
```

### Real backend — frontend with actual login screen (requires layers to be implemented)

```bash
cp .env.dev.example .env
# Edit .env as needed (DATABASE_URL and JWT_* already have dev defaults)

dotnet run --project DataAccessLayer3/Db/InitDb/InitDb.csproj   # create DB from schema.sql

# Terminal 1 — Razor Pages UI
dotnet run --project PresentationLayer1   # → http://localhost:5080

# Terminal 2 — notification worker
dotnet run --project Worker
```

`MOCK_LOGIN=false` in `.env.dev.example` shows the real email + password login form.

`GET http://localhost:5080/health` returns `{ "status": "ok" }`. First build runs
`dotnet restore`; bump package versions in the `.csproj` files if your SDK wants
newer ones.
