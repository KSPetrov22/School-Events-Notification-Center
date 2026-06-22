# Database (SQLite)

Local SQLite database for development. To move to PostgreSQL or MySQL later, keep the same table/column names and switch the EF Core provider + connection string.

## Files

| File | Purpose |
|------|---------|
| `schema.sql` | Full DDL (tables, indexes, constraints) |
| `seed.sql` | Dev users + sample events (optional) |
| `school_events.db` | Ready-to-use database file |
| `InitDb/` | Small tool to recreate the `.db` from SQL scripts |

## Recreate the database

```powershell
dotnet run --project DataAccessLayer3/Db/InitDb/InitDb.csproj
```

## Connection string

In `.env`:

```
DATABASE_URL=Data Source=DataAccessLayer3/Db/school_events.db
```

Use an absolute path if the API/worker run from a different working directory:

```
DATABASE_URL=Data Source=C:\Users\kaloy\source\repos\School-Events-Notification-Center\DataAccessLayer3\Db\school_events.db
```

## Seed accounts (dev only)

All seed users share password: **`password`**

| Email | Role |
|-------|------|
| organizer1@school.local | ORGANIZER |
| organizer2@school.local | ORGANIZER |
| student1@school.local | STUDENT |
| student2@school.local | STUDENT |
| student3@school.local | STUDENT |

## Tables

- `users` — auth + roles (STUDENT / ORGANIZER)
- `events` — DRAFT / PUBLISHED / CANCELLED
- `registrations` — CONFIRMED / WAITLISTED / CANCELLED + FIFO waitlist
- `notification_jobs` — async outbox for the worker
- `notification_log` — email delivery audit
- `badges` — one badge per published event
- `attendances` — check-in records
- `user_badges` — badges earned by students
