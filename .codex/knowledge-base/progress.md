# Frontend Scaffold Progress

## Architecture Design

- Frontend target: `PresentationLayer1` using C# Razor Pages.
- Static assets under `PresentationLayer1/wwwroot`.
- Mock backend target: separate root-level `MockServer` project (port 5090).
- Mock storage: SQLite file initialized from `DataAccessLayer3/Db/schema.sql`.
- Razor Pages call the mock server through typed C# client services (`IMockApiClient`).
- Future backend swap requires only configuration and client changes, not UI rewrites.

## Plan Progress

- [x] Extracted frontend requirements from `task.md`.
- [x] Chose Razor Pages for the C# frontend model.
- [x] Chose a separate mock server instead of browser-only mocks.
- [x] Chose SQLite storage initialized from `schema.sql`.
- [x] Create `MockServer` project with full REST surface.
- [x] Add mock REST endpoints (events CRUD, publish/cancel, registrations, waitlist, `/registrations/me`).
- [x] Add Razor Pages frontend scaffold — all student and organizer flows.
- [x] Wire frontend client services to mock server.
- [x] Verified student and organizer flows against a running `net10.0` SDK.

## Implementation State

The frontend and mock stack are fully operational. All Razor Pages for student and organizer
workflows are wired to the MockServer and verified running on .NET 10.

**Features implemented beyond the original scaffold:**

- Local-timezone datetime display via `<time class="local-time">` + `Intl` JS API.
- Copy-link and public-link buttons on published events (organizer cards and student detail view).
- Location display in event cards and detail pages.
- FULL capacity pill on organizer event cards when an event is at capacity.
- Graceful handling of editing a cancelled event: redirects to index with a flash message
  instead of returning a raw 404.
- TempData error/message flash notifications in `_Layout.cshtml`.
- `wwwroot/app.js` for all browser-side enhancements (time conversion, clipboard).

**MOCK_LOGIN toggle:**

`MOCK_LOGIN=false` (default everywhere) shows the real email + password login form.
Set `MOCK_LOGIN=true` via `.env.mockdev.example` to activate the mock dropdown of seeded accounts.

## Env File Structure

| File | Purpose |
|---|---|
| `appsettings.Development.json` | Provides `DATABASE_URL` and `MOCK_LOGIN=false` defaults; mock stack needs no `.env` |
| `.env.mockdev.example` | Copy to `.env` to enable mock login dropdown (`MOCK_LOGIN=true`) |
| `.env.dev.example` | Copy to `.env` for real backend local dev (SQLite, MailHog, dev JWT secret) |

## Run Notes

### Mock stack (working today)

```bash
cp .env.mockdev.example .env
dotnet run --project MockServer           # → http://localhost:5090
dotnet run --project PresentationLayer1   # → http://localhost:5080
```

Seeded mock accounts (no password):

- `student1@school.local` / `student2@school.local` — Student
- `organizer1@school.local` / `organizer2@school.local` — Organizer

### Real backend (TODO — layers not yet implemented)

```bash
cp .env.dev.example .env
dotnet run --project DataAccessLayer3/Db/InitDb/InitDb.csproj
dotnet run --project PresentationLayer1   # → http://localhost:5080
dotnet run --project Worker
```

## Known Gaps / Next Steps

- `BusinessLogicLayer2/Services/` and `DataAccessLayer3/Models/` are still empty (`TODO`).
- `Worker/NotificationWorker.cs` is a poll-loop skeleton only.
- Mock auth uses `X-Mock-User-Id` header; real JWT auth wiring is not done.
- MockServer is temporary — the real REST surface should mirror its contract.
