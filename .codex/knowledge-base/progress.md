# Frontend Scaffold Progress

## Architecture Design

- Frontend target: `PresentationLayer1` using C# Razor Pages.
- Static assets remain under `PresentationLayer1/wwwroot`.
- Mock backend target: separate root-level `MockServer` project.
- Mock storage: SQLite file initialized from `DataAccessLayer3/Db/schema.sql`.
- Razor Pages call the mock server through C# client services.
- Future backend swap should require configuration/client changes, not UI rewrites.

## Current Plan

Implement a Razor Pages frontend scaffold for student and organizer workflows
while the real backend is absent. Provide a mock server that exposes
backend-shaped REST endpoints and persists demo data in SQLite.

## Plan Progress

- [x] Extracted frontend requirements from `task.md`.
- [x] Chose Razor Pages for the C# frontend model.
- [x] Chose a separate mock server instead of browser-only mocks.
- [x] Chose SQLite storage initialized from `schema.sql`.
- [x] Create `MockServer` project.
- [x] Add mock REST endpoints.
- [x] Add Razor Pages frontend scaffold.
- [x] Wire frontend client services to mock server.
- [ ] Verify student and organizer flows against a running `net10.0` SDK.

## Implementation State

Frontend scaffold implementation is complete for the planned first pass. The
Razor Pages UI, typed mock API client, separate mock server, SQLite-backed mock
storage, and seeded student/organizer flows have been added.

Local verification is blocked by the installed SDK version: this machine has
.NET 8 while the solution targets `net10.0`. `dotnet build SchoolEvents.slnx`
also requires newer tooling that understands `.slnx`.

## Run Notes

With a .NET SDK that supports `net10.0` and `.slnx`:

```powershell
dotnet run --project MockServer
dotnet run --project PresentationLayer1
```

Then open `http://localhost:5080`. The mock server listens on
`http://localhost:5090`, and `PresentationLayer1` reads that from
`MockApiBaseUrl`.

Seeded mock accounts:

- `student1@school.local`
- `student2@school.local`
- `organizer1@school.local`

## Known Gaps / Next Decisions

- Local machine currently has .NET 8 SDK, while projects target `net10.0`.
- Do not retarget projects unless explicitly requested.
- Mock auth uses seeded users and mock session values.
- Mock server is temporary and should mimic backend REST contracts closely
  enough to swap out later.
