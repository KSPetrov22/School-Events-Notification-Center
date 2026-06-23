# Backend Mandatory Requirements

Extracted from `task.md`.

## Core Backend Scope

Build the mandatory backend for a school events system where organizers create
and publish capacity-limited events, students register, full events use a FIFO
waitlist, and registration-related side effects are processed asynchronously by
a separate worker.

## User Roles

### Student

- See published events and basic details such as title, time, place/link,
  capacity, and whether the event is full.
- Register for an event.
  - If capacity is available, registration becomes `CONFIRMED`.
  - If the event is full, registration becomes `WAITLISTED` with a position.
- Cancel own registration, using a simple documented rule.
- See only own registrations.

### Organizer

- Create and edit events in `DRAFT`.
- Publish events so students can see them.
- Set title, description, start/end or single datetime, capacity >= 1, and
  optional location/URL text.
- Cancel or close an event, with a documented rule for existing registrations.
- List registrations and waitlist entries for own events only.

## Mandatory Features

### 1. Authentication And Roles

- User registration and login.
- Role-based access control for student vs organizer.
- Return `403` or equivalent when a user tries to access forbidden operations.
- Students may see only their own registrations.

### 2. Event Catalog

- Organizers can create and edit events in `DRAFT`.
- Events include at least:
  - `title`
  - `description`
  - `starts_at` / `ends_at`, or a single start time
  - `capacity`, integer >= 1
  - optional location or URL as plain text
- Publish flow: `DRAFT` -> `PUBLISHED`.
- Students can list and read only `PUBLISHED` events.
- Student responses should support showing capacity, full status, and counts as
  designed.
- Organizers can cancel or close events.

### 3. Registration And Waitlist

- Students register for published events.
- Registration becomes `CONFIRMED` if capacity is available.
- Registration becomes `WAITLISTED` if the event is full.
- Waitlist ordering is FIFO, for example by `registered_at`.
- No overbooking of confirmed seats, including under concurrent registration
  requests.
- At most one active registration per user/event.
- Students can cancel own registration using a simple documented rule.
- When a confirmed seat is freed, automatically promote the next waitlisted
  student.

### 4. Organizer Registration Operations

- Organizers can list confirmed registrations for their own events.
- Organizers can list waitlist entries for their own events in deterministic
  FIFO order.

### 5. Async Notifications

- After the API successfully persists registration-related changes, it enqueues
  notification work so HTTP requests stay fast.
- API and worker must be separate runnable processes.
- Implement at least three distinct job/event types end-to-end:
  - API creates the job.
  - Queue stores the job.
  - Worker processes the job.
  - Email is sent, or development log-only behavior is used.
- Example event/job types:
  - `RegistrationConfirmed`
  - `RegistrationWaitlisted`
  - `WaitlistPromoted`
  - `RegistrationCancelled`
  - `EventPublished`
  - `EventCancelled`

## Backend Architecture Requirements

### Database Integration

- Use a relational database such as PostgreSQL or MySQL for users, events,
  registrations, and related data.

### REST API Development

- Implement endpoints for authentication, events, registrations, organizer
  registration lists, and student self-registration lists.
- Use proper HTTP methods and status codes.
- Return JSON validation errors.
- Require authentication on protected routes.
- Apply authorization checks on every sensitive operation.
- Use consistent error responses.

### Event-Driven Processing

- API and worker are separate runnable processes.
- The API is the producer that enqueues jobs after successful writes.
- The worker is the consumer that pulls jobs and performs the side effect.
- A single deployable API is acceptable; multiple microservices are not
  required.

## REST Endpoint Guidance

### Authentication

- `POST /register` - create user. Role creation should be restricted and
  documented, for example only students through the API and organizers via seed.
- `POST /login` - return token/session.
- `POST /logout` - only if sessions are used.

### Events

- `POST /events` - organizer creates `DRAFT`.
- `GET /events` - students see published only; organizers see own drafts and
  published events.
- `GET /events/{id}` - details plus counts as appropriate for role.
- `PUT /events/{id}` - organizer owns event; only if still editable.
- `POST /events/{id}/publish` - draft to published.
- `POST /events/{id}/cancel` - cancel event and enqueue cancellation handling if
  implemented.

### Registrations

- `POST /events/{id}/registrations` - student registers and domain events are
  enqueued.
- `DELETE /registrations/{id}` - student cancels own registration; may promote
  next waitlisted student; domain events are enqueued.
- `GET /registrations/me` - student sees own registrations, statuses, and
  waitlist positions.
- `GET /events/{id}/registrations` - organizer sees confirmed registrations.
- `GET /events/{id}/waitlist` - organizer sees ordered waitlist.

### Optional Profile

- `GET /users/me` - current user profile.
- `PUT /users/me` - update own display name or email if allowed.

### Optional Notification Jobs

- `GET /notification-jobs/{id}` - internal/debug or protected by a documented
  rule.
- `GET /notification-jobs` - optional list with filters such as `event_id`.

## Domain Event And Queue Guidance

Domain events should be facts that already happened, such as registration
confirmation. They should carry ids and type values, not free-form client text.
The worker should load names and emails from the database using those ids.

Recommended table-as-queue pattern:

1. HTTP handler validates input and checks auth/role.
2. Start DB transaction.
3. Change business state.
4. Insert queue row or rows in the same transaction.
5. Commit.
6. Return quickly without calling email APIs from the request path.

## Idempotency And Retries

Workers may retry, so duplicate processing must be safe. Use one or both:

- Unique `idempotency_key`, for example
  `registration_id:RegistrationConfirmed`.
- Worker-side check that a notification for the same registration/type was
  already logged.

