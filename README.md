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
PresentationLayer1/        ASP.NET Core host (= reference main/app.py)
  Program.cs               entry point: load config, wire layers, serve UI + API
  Controllers/             (empty) HTTP controllers go here
  wwwroot/                 static web UI (= reference static/): index.html, app.js, style.css
  DependencyInjection?     none — wiring is in BusinessLogicLayer2
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
.env.example               copy to .env
```

(Empty folders are kept in git with a `.gitkeep` placeholder; delete it once you
add real files.)

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
- JWT Bearer auth · BCrypt password hashing · MailKit (SMTP) · DotNetEnv
- Static web UI: plain HTML/JS/CSS from `wwwroot/`

## Run (once there's something to run)

```bash
cp .env.example .env                        # set DATABASE_URL etc.
dotnet run --project PresentationLayer1      # API + web UI → http://localhost:5080
dotnet run --project Worker                  # background worker (separate process)
```

`GET http://localhost:5080/health` returns `{ "status": "ok" }`. First build runs
`dotnet restore`; bump package versions in the `.csproj` files if your SDK wants
newer ones.
